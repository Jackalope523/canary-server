using Core.Boundaries;
using Core.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Core.Controls
{
	internal class EventDirector : AbstractDirector, IEventOperations
	{
		public EventDirector(CoreTerminal terminal) : base(terminal) { }

		public async Task<EventShard> GetEventInformationAsync(ulong userId, ulong eventId)
        {
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException("User is unable to view event."); }

			return targetEvent.ToEventShard();
		}

		public async Task<List<EventThinSlice>> GetEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUser(userId);
			var nearbyEvents = Events.FindEvents(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			await RemoveInaccessibleEventsAsync(user, nearbyEvents);

			return nearbyEvents;
		}

		public async Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUser(userId);
			var nearbyEvents = Events.FindEvents(latitude, longitude, distance);

			// Remove inaccessible events and events with a large difference between event and user interest
			await RemoveUnattractiveEventsAsync(user, nearbyEvents, 1f);

			return nearbyEvents;
		}

		public async Task<EventShard> CreateEventAsync(ulong userId,
			string eventName, string eventDescription, DateTimeOffset startTime,
			double latitude, double longitude,
			int? groupMinimum, int? groupMaximum)
		{
			// Check if user is already at an event
			await ThrowIfUserAtEvent(userId);
			var user = await GetUser(userId);
			
			// Check if user can host
			if (!user.CanHost)
			{
				throw new InvalidUserException("User cannot host.\n" +
				$"Account Status: {user.AccountStatus}");
			}

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
            var newEvent = Events.CreateEvent(user.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.GroupMinimum, eventStub.GroupMaximum, user.Character.ToCharacter());

			return newEvent;
		}

		public async Task EditEventAsync(ulong userId, ulong eventId,
			string eventDescription = "", bool? isOpen = null)
		{
			var targetEvent = await GetEvent(eventId);

			// Verify that user is event host
			if (!await targetEvent.IsModifiableBy(userId))
			{ throw new InvalidEventException("User is unable to edit event."); }

			// Verify that event is still active
			if (targetEvent.EndTime.HasValue)
			{ throw new InvalidEventException("Unable to edit event, event has ended."); }

			targetEvent.Description = eventDescription;
			targetEvent.IsOpen = isOpen ?? targetEvent.IsOpen;

			targetEvent.ValidateAndNormalise();

			List<(string Property, object Value)> edits = new();

			// Track individual edits
			if (eventDescription != "")
			{
				edits.Add((nameof(EventShard.Description), targetEvent.Description));
			}
			if (isOpen.HasValue)
			{
				edits.Add((nameof(EventShard.IsOpen), targetEvent.IsOpen));
			}

			// Push update
			Events.UpdateEvent(targetEvent.Id, edits);
		}

		public async Task WatchEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException($"User is unable to view or join event.\nAccount Status: {user.AccountStatus}"); }

			// TODO Can user watch events if time conflict

			var userIntention = Events.GetUserState(userId, eventId);
			bool success;

			// Ensure correct state transition
			if (!userIntention.HasValue)
			{
				// Try to add user to the event
				success = Events.SetUserState(userId, eventId, EventUserState.Watching);
				if (!success)
				{ throw new UnexpectedFailureException("Could not watch event."); }
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not watch event, user currently {userIntention.Value} event."); }
		}

		public async Task UnwatchEventAsync(ulong userId, ulong eventId)
		{
			var userIntention = Events.GetUserState(userId, eventId);
			bool success;

			// Ensure correct state transition
			if (userIntention.HasValue && userIntention.Value.Equals(EventUserState.Watching))
			{
				// Try to remove user from event
				success = Events.RemoveUser(userId, eventId);
				if (!success)
				{ throw new UnexpectedFailureException("Could not unwatch event."); }
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not unwatch event, user currently {userIntention.Value} event."); }
		}

		public async Task JoinEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException($"User is unable to view or join event.\nAccount Status: {user.AccountStatus}"); }

			// Check if event is open
			if (!targetEvent.IsOpen)
			{ throw new InvalidEventException("Event is closed."); }

			// TODO Check if conflicting and check if currently active and there

			await ThrowIfUserAtEvent(userId);

			// Try to add user to the event
			bool success = Events.SetUserState(userId, eventId, EventUserState.Attending);
			if (!success)
			{ throw new UnexpectedFailureException("Could not attend event."); }
		}

		public async Task LeaveEventAsync(ulong userId, ulong eventId)
		{
			var targetEvent = await GetEvent(eventId);

			// Check if user is the host
			if (targetEvent.Host.Id.Equals(userId))
			{ throw new InvalidUserException("Host cannot leave the event."); }

			// Get the user's current status
			var userIntention = Events.GetUserState(userId, eventId);
			bool success;

			if (userIntention.Equals(EventUserState.Present))
			{
				// Try to remove user from event
				success = Events.SetUserState(userId, eventId, EventUserState.Left);
				if (!success)
				{ throw new UnexpectedFailureException("Could not leave event."); }
			}
			else if (userIntention.Equals(EventUserState.Attending))
			{
				// Try to remove user from event
				success = Events.RemoveUser(userId, eventId);
				if (!success)
				{ throw new UnexpectedFailureException("Could not unattend event."); }
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not leave event, user currently {userIntention.Value} event."); }
		}

		public async Task EndEventAsync(ulong userId, ulong eventId)
		{
			var targetEvent = await GetEvent(eventId);

			// Check if the user is able to end the event
			if (!await targetEvent.IsModifiableBy(userId))
			{ throw new InvalidUserException("User does not have permissions to end event."); }

			// Try to end to event
			bool success = Events.EndEvent(eventId);
            if (!success)
            { throw new UnexpectedFailureException("Could not end event."); }

			// Update all participants' vectors
			foreach (var guestDetails in Events.GetGuestHistory(targetEvent.Id))
			{
				User guest = new(guestDetails.User);

				guest.CalculateCharacter(targetEvent, guestDetails.Left.Value - guestDetails.Joined);

				Accounts.UpdateUser(guest.Id, new() { (nameof(UserShard.Character), guest.Character) });
			}
        }

		public async Task<List<UserSilhouette>> GetAttendeesAsync(ulong userId, ulong eventId)
		{
			Event targetEvent = new(eventId);

			// Check if user attended
			if (await targetEvent.IsAttendedBy(userId))
			{
				return targetEvent.Guests;
			}
			else
			{
				// Retrieve user's friends
				var friends = Profiles.GetFriends(userId);
				List<UserSilhouette> friendAttendees = new();

				// Check if any friends are attending
				foreach (var friend in friends)
				{
					if (await targetEvent.WasAttendedBy(friend.Id))
					{
						friendAttendees.Add(friend);
					}
				}

				return friendAttendees;
			}
		}


		internal async Task<List<UserSilhouette>> GetGuestsInternalAsync(ulong eventId)
		{
			return Events.GetGuests(eventId);
		}

		internal async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetAllGuestsInternalAsync(ulong eventId)
		{
			return Events.GetGuestHistory(eventId);
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

		internal async Task<EventShard> GetCurrentEventAsync(ulong userId)
		{
			return Events.FindCurrentEventForUser(userId);
		}


		private async Task ThrowIfUserAtEvent(ulong userId)
		{
			User user = new(userId);
			await user.SyncCurrentEvent();

			if (user.IsAtEvent)
			{ throw new InvalidUserException("User is currently attending an event."); }
		}
    }
}
