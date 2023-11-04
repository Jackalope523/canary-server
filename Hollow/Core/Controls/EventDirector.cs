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

		public async Task<EventShard> GetEventInformationAsync(Guid userID, Guid eventID)
        {
			var user = await GetUser(userID);
			var targetEvent = await GetEvent(eventID);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException("User is unable to view event."); }

			return targetEvent.ToThinEvent();
		}

		public async Task<List<EventThinSlice>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var user = await GetUser(userID);
			var nearbyEvents = Events.FindEvents(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			await RemoveInaccessibleEventsAsync(user, nearbyEvents);

			return nearbyEvents;
		}

		public async Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var user = await GetUser(userID);
			var nearbyEvents = Events.FindEvents(latitude, longitude, distance);

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
			var user = await GetUser(userID);
			
			// Check if user can host
			if (!user.CanHost)
			{ throw new InvalidUserException("User cannot host.\n" +
				$"Account Status: {user.AccountStatus}\n" +
				$"Cooldown: {(user.HostCooldown.HasValue ? user.HostCooldown.Value : "none")}"); }

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

			user.EventCreated();

			// On success, set host cooldown
			Accounts.UpdateUser(user.Id, new() { ("HostCooldown", user.HostCooldown) });

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

			List<(string Property, object Value)> edits = new();

			// Track individual edits
			if (eventDescription != "")
			{
				edits.Add(("Description", targetEvent.Description));
			}
			if (isOpen.HasValue)
			{
				edits.Add(("IsOpen", targetEvent.IsOpen));
			}

			// Push update
			Events.UpdateEvent(targetEvent.Id, edits);
		}

		public async Task JoinEventAsync(Guid userID, Guid eventID)
		{
			var user = await GetUser(userID);
			await ThrowIfUserAtEvent(userID);

			var targetEvent = await GetEvent(eventID);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(user))
			{ throw new InvalidEventException("User is unable to view event."); }

			// Check if event is open
			if (!targetEvent.IsOpen)
			{ throw new InvalidEventException("Event is closed."); }

			// Try to add user to the event
			bool success = Events.AddUserToEvent(userID, eventID);
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
			bool success = Events.RemoveUserFromEvent(userID, eventID);
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
			bool success = Events.EndEvent(eventID);
            if (!success)
            { throw new UnexpectedFailureException("Could not end event."); }

			// Update all participants' vectors
			foreach (var guestDetails in Events.GetGuestHistory(targetEvent.Id))
			{
				User guest = new(guestDetails.User);

				guest.CalculateCharacter(targetEvent, guestDetails.Left.Value - guestDetails.Joined);

				Accounts.UpdateUser(guest.Id, new() { ("Character", guest.Character) });
			}
        }

		public async Task<List<UserSilhouette>> GetAttendeesAsync(Guid userID, Guid eventID)
		{
			Event targetEvent = new(eventID);

			// Check if user attended
			if (!await targetEvent.IsAttendedBy(userID))
			{
				// Retrieve user's friends
				var friends = Profiles.GetFriends(userID);
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

		internal async Task<List<UserSilhouette>> GetAttendeesInternalAsync(Guid eventID)
		{
			return Events.GetGuestList(eventID);
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
			return Events.FindCurrentEventForUser(userID);
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
