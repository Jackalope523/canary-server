using Server.Boundaries;
using Server.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Server.Controls
{
	public class EventManager : IEventOperations
	{
		internal static EventManager Manager { get; private set; }

		private IAccountDatabase accounts;
		private IEventDatabase events;

		public EventManager(IAccountDatabase accountDatabase, IEventDatabase eventDatabase)
		{
			Manager = this;

			accounts = accountDatabase;
			events = eventDatabase;
		}

		public async Task<EventShard> GetEventInformationAsync(Guid userID, Guid eventID)
        {
			var user = await AccountManager.Manager.GetUser(userID);
			var targetEvent = await GetEvent(eventID);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException("User is unable to view event."); }

			return targetEvent.ToThinEvent();
		}

		public async Task<List<EventThinSlice>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var user = await AccountManager.Manager.GetUser(userID);
			var nearbyEvents = events.FindEvents(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			await RemoveInaccessibleEventsAsync(user, nearbyEvents);

			return nearbyEvents;
		}

		public async Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var user = await AccountManager.Manager.GetUser(userID);
			var nearbyEvents = events.FindEvents(latitude, longitude, distance);

			// Remove inaccessible events and events with a large difference between event and user interest
			await RemoveUnattractiveEventsAsync(user, nearbyEvents, 1f);

			return nearbyEvents;
		}

		public async Task<EventShard> CreateEventAsync(Guid userID,
			string eventName, string eventDescription, DateTimeOffset startTime,
			double latitude, double longitude,
			int? groupMinimum, int? groupMaximum)
		{
			// Check if user is already at an event
			await ThrowIfUserAtEvent(userID);
			var user = await AccountManager.Manager.GetUser(userID);
			
			// Check if user can host
			if (!user.CanHost)
			{ throw new InvalidUserException("User cannot host."); }

			// Create event
			Event eventStub = new()
			{
				Name = eventName,
				Description = eventDescription,
				StartTime = startTime,
				Location = new() { Latitude = latitude, Longitude = longitude },
				GroupMinimum = groupMinimum ?? 0,
				GroupMaximum = groupMaximum ?? 0
			};

			// Validate event
			bool valid = eventStub.ValidateAndNormalise();
            if (!valid)
            { throw new InvalidInformationException("Invalid event details provided."); }

            // Try to create an event
            var newEvent = events.CreateEvent(userID, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.GroupMinimum, eventStub.GroupMaximum, user.Character.ToCharacter());
			return newEvent;
		}

		public async Task EditEventAsync(Guid userID, Guid eventID,
			string eventDescription = "", bool? isOpen = null)
		{
			var targetEvent = await GetEvent(eventID);

			// Verify that user is event host
			if (!await targetEvent.IsModifiableBy(userID))
			{ throw new InvalidEventException("User is unable to edit event."); }

			// Verify that event is still active
			if (targetEvent.EndTime.HasValue)
			{ throw new InvalidEventException("Unable to edit event, event has ended."); }

			targetEvent.Description = eventDescription;
			targetEvent.IsOpen = isOpen ?? targetEvent.IsOpen;

			targetEvent.ValidateAndNormalise();

			// Update individual attributes
			if (eventDescription != "")
			{
				events.UpdateDescription(eventID, targetEvent.Description);
			}
			if (isOpen.HasValue)
			{
				events.UpdateStatus(eventID, targetEvent.IsOpen);
			}
		}

		public async Task JoinEventAsync(Guid userID, Guid eventID)
		{
			var user = await AccountManager.Manager.GetUser(userID);
			await ThrowIfUserAtEvent(userID);

			var targetEvent = await GetEvent(eventID);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException("User is unable to view event."); }

			// Check if event is open
			if (!targetEvent.IsOpen)
			{ throw new InvalidEventException("Event is closed."); }

			// Try to add user to the event
			bool success = events.AddUserToEvent(userID, eventID);
			if (!success)
			{ throw new UnexpectedFailureException("Could not join event."); }
		}

		public async Task LeaveEventAsync(Guid userID, Guid eventID)
		{
			var targetEvent = await GetEvent(eventID);

			// Check if user is the host
			if (targetEvent.Host.Id.Equals(userID))
			{ throw new InvalidUserException("Host cannot leave the event."); }

			// Try to remove user from event
			bool success = events.RemoveUserFromEvent(userID, eventID);
            if (!success)
            { throw new UnexpectedFailureException("Could not leave event."); }
        }

		public async Task EndEventAsync(Guid userID, Guid eventID)
		{
			var targetEvent = await GetEvent(eventID);

			// Check if the user is able to end the event
			if (!await targetEvent.IsModifiableBy(userID))
			{ throw new InvalidUserException("User does not have permissions to end event."); }

			// Try to end to event
			bool success = events.EndEvent(eventID);
            if (!success)
            { throw new UnexpectedFailureException("Could not end event."); }

			// Update all participants' vectors
			foreach (var guestDetails in events.GetGuestHistory(targetEvent.Id))
			{
				User guest = new(guestDetails.User);

				guest.CalculateCharacter(targetEvent, guestDetails.Left.Value - guestDetails.Joined);

				accounts.UpdateUserCharacter(guest.Id, guest.Character.Extraversion,
					guest.Character.Athleticism, guest.Character.Chaoticness,
					guest.Character.Competitiveness, guest.Character.Industriousness,
					guest.Character.NightOwl, guest.Character.Openness);
			}
        }

		public async Task<List<UserSilhouette>> GetAttendeesAsync(Guid userID, Guid eventID)
		{
			Event targetEvent = new(eventID);

			// Check if user attended
			if (!await targetEvent.IsAttendedBy(userID))
			{
				// Retrieve user's friends
				var friends = accounts.GetFriends(userID);
				List<UserSilhouette> friendAttendees = new();

				// Check if any friends are attending
				foreach (var friend in friends)
				{
					if (await targetEvent.IsAttendedBy(friend.Id))
					{
						friendAttendees.Add(friend);
					}
				}

				// Check if user had friends that attended
				if (friendAttendees.Count == 0)
				{ throw new InvalidUserException("User did not attend event."); }

				return friendAttendees;
			}

			return targetEvent.Attendees;
		}

        public async Task ReportEventAsync(Guid userID, Guid eventID, Guid hostId,
			EventReportType reportType, string reportDetails)
		{
			var targetEvent = await GetEvent(eventID);
			events.ReportEvent(userID, eventID, hostId, reportType, reportDetails);

			// Check if action is to be taken
			if (await targetEvent.Reported())
			{
				// Threshold hit, end event
				await EndEventAsync(targetEvent.Host.Id, eventID);

				// Compute host's standing
				var user = await AccountManager.Manager.GetUser(targetEvent.Host.Id);
				var status = await user.EventReported();

				// Check if host should be punished
				if (user.AccountStatus != status)
				{
					accounts.UpdateAccountStatus(user.Id, status);
				}
			}
		}

		public async Task<List<EventPost>> GetEventPostsAsync(Guid userID, Guid eventID)
		{
			var user = await AccountManager.Manager.GetUser(userID);
			Event targetEvent = new(eventID);

			// Ensure user can see the event
			if (!await targetEvent.IsAttendedBy(user))
			{ throw new InvalidEventException("User did not attend or is not attending event."); }

			var eventPosts = events.GetPostsForEvent(eventID);

			return eventPosts;
		}

		public async Task<EventPost> AddPostAsync(Guid userID, Guid eventID, string imageURL)
		{
			User user = new(userID);
			var targetEvent = await GetEvent(eventID);

			// Ensure the user can post to the event
			if (!await targetEvent.IsAttendedBy(user))
			{ throw new InvalidEventException("User is not attending event."); }

			// Ensure event is still running
			if (targetEvent.EndTime.HasValue)
			{ throw new InvalidEventException("Event has already ended."); }
			
			// Try to post
			var userPost = events.AddPost(eventID, userID, DateTimeOffset.UtcNow, imageURL);

			return userPost;
		}

		public async Task RemovePostAsync(Guid userID, Guid postID)
		{
			var eventPost = events.GetPost(postID);
			
			// Check if user can delete post
			if (!eventPost.UserId.Equals(userID))
			{ throw new InvalidUserException("User cannot remove post."); }

			events.RemovePost(postID);
		}

		public async Task RatePostAsync(Guid userID, Guid postID, UserRating rating)
		{
			User user = new(userID);
			var eventOfPost = await GetEvent(events.GetPost(postID).EventId);
			
			// Check if user can interact with post
			if (!await eventOfPost.IsAttendedBy(user))
			{ throw new InvalidUserException("User cannot interact with post."); }

			// Check if removing a rating
			if (rating != UserRating.Remove)
			{
				events.RatePost(userID, postID, rating);
			}
			else
			{
				events.RemovePostRating(postID, userID);
			}
		}

		public async Task<(int Depth, List<EventHeader> Headers, List<EventPost> Posts)>
			GetUserFeedAsync(Guid userID, int depth = 0, List<Guid> exclusionList = null)
		{
			User user = new(userID);
			exclusionList ??= new();
			Dictionary<Guid, EventHeader> eventHeaders = new();

			// Retrieve friend-populated event posts after a specified time excluding previously viewed events
			DateTimeOffset depthCharge = DateTimeOffset.UtcNow - TimeSpan.FromDays(1 + depth);
			var friendPosts = events.GenerateFeedForUser(user.Id, depthCharge, exclusionList);

			// Get the respective event headers for the posts
			foreach (EventPost post in friendPosts)
			{
				// Add event header if it does not yet exist
				if (!eventHeaders.ContainsKey(post.EventId))
				{
					Event postEvent = new(events.FindEvent(post.EventId));

					eventHeaders.Add(post.EventId, postEvent.ToEventHeader(post.TimePosted));
				}
				// Update event header active time if post is more recent
				else if (eventHeaders[post.EventId].LastActiveTime < post.TimePosted)
				{
					eventHeaders[post.EventId] = new(post.EventId,
						eventHeaders[post.EventId].Name,
						eventHeaders[post.EventId].IsActive,
						post.TimePosted);
				}
			}

			return (depth, eventHeaders.Values.ToList(), friendPosts);
		}



		internal async Task<Event> GetEvent(Guid eventID)
		{
			return new(events.FindEvent(eventID));
		}

		internal async Task<List<UserSilhouette>> GetAttendeesInternalAsync(Guid eventID)
		{
			return events.GetGuestList(eventID);
		}

		internal async Task<List<EventShard>>
			RemoveInaccessibleEventsAsync(User user, List<EventShard> events)
		{
			foreach (EventShard e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(user))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<List<EventThinSlice>>
			RemoveInaccessibleEventsAsync(User user, List<EventThinSlice> events)
		{
			foreach (EventThinSlice e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(user))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<List<EventThinSlice>>
			RemoveUnattractiveEventsAsync(User user, List<EventThinSlice> events, float maximumAngle)
		{
			foreach (EventThinSlice e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(user))
				{ events.Remove(e); continue; }

				if (CharacterVector.AngleBetweenAffected(user.Character, targetEvent.Character) > maximumAngle)
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<EventShard> GetCurrentEventAsync(Guid userID)
		{
			return events.FindCurrentEvent(userID);
		}

		internal async Task<List<EventReport>> GetEventReportsAsync(Guid eventID)
		{
			return events.GetReportsAboutEvent(eventID);
		}

		internal async Task<List<EventPost>> GetEventPostsAsync(Guid eventID)
		{
			return events.GetPostsForEvent(eventID);
		}


		private async Task ThrowIfUserAtEvent(Guid userID)
		{
			User user = new(userID);
			await user.SyncCurrentEvent();

			if (user.IsAtEvent)
			{ throw new InvalidUserException("User is currently attending an event."); }
		}

       
    }
}
