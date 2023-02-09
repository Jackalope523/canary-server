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

		public async Task<ThinEvent> GetEventInformationAsync(Guid userID, Guid eventID)
        {
			var targetEvent = await GetEvent(eventID);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(userID))
			{ throw new InvalidEventException("User is unable to view event."); }

			return targetEvent.ToThinEvent();
		}

		public async Task<List<ThinnerEvent>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var nearbyEvents = events.FindEvents(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			await RemoveInaccessibleEventsAsync(userID, nearbyEvents);

			return nearbyEvents;
		}

		public async Task<List<ThinnerEvent>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var nearbyEvents = await GetEventsInAreaAsync(userID, latitude, longitude, distance);

			// TODO User interest weighting here

			return nearbyEvents;
		}

		public async Task<ThinEvent> CreateEventAsync(Guid userID,
			string eventName, string eventDescription, string eventType,
			DateTimeOffset startTime, double latitude, double longitude,
			int? groupMinimum, int? groupMaximum)
		{
			await ThrowIfUserAtEvent(userID);

			// TODO Validate event
			// Try to create an event
			var newEvent = events.CreateEvent(userID, eventName, eventDescription, eventType,
				startTime, latitude, longitude,
				groupMinimum ?? 0, groupMaximum ?? 0);
			return newEvent;
		}

		public async Task EditEventAsync(Guid userID, Guid eventID,
			string eventDescription = "", string eventType = "",
			bool? isOpen = null)
		{
			var targetEvent = await GetEvent(eventID);

			// Verify that user is event host
			if (!await targetEvent.IsModifiableBy(userID))
			{ throw new InvalidEventException("User is unable to edit event."); }

			// Verify that event is still active
			if (targetEvent.EndTime.HasValue)
			{ throw new InvalidEventException("Unable to edit event, event has ended."); }

			// TODO Verify updates are valid
			// Update individual attributes
			if (eventDescription != "")
			{
				events.UpdateDescription(eventID, eventDescription);
			}
			if (eventType != "")
			{
				events.UpdateType(eventID, eventType);
			}
			if (isOpen.HasValue)
			{
				events.UpdateStatus(eventID, isOpen.Value);
			}
		}

		public async Task JoinEventAsync(Guid userID, Guid eventID)
		{
			await ThrowIfUserAtEvent(userID);

			var targetEvent = await GetEvent(eventID);

			// Check if user is allowed to view event
			if (!await targetEvent.IsVisibleTo(userID))
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
			// TODO Is Host logic

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
        }

		public async Task<List<ThinnerUser>> GetAttendeesAsync(Guid userID, Guid eventID)
		{
			Event targetEvent = new(eventID);

			// Check if user attended
			if (!await targetEvent.IsAttendedBy(userID))
			{
				// Retrieve user's friends
				var friends = accounts.GetFriends(userID);
				List<ThinnerUser> friendAttendees = new();

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

		public async Task ReportEventAsync(Guid userID, Guid eventID, EventReport reportType, string reportDetails)
		{
			events.ReportEvent(userID, eventID, reportType, reportDetails);
		}



		internal async Task<Event> GetEvent(Guid eventID)
		{
			return new(events.FindEvent(eventID));
		}

		internal async Task<List<ThinnerUser>> GetAttendeesInternalAsync(Guid eventID)
		{
			return events.GetGuestList(eventID);
		}

		internal async Task<List<ThinEvent>> RemoveInaccessibleEventsAsync(Guid userID, List<ThinEvent> events)
		{
			foreach (ThinEvent e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(userID))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<List<ThinnerEvent>> RemoveInaccessibleEventsAsync(Guid userID, List<ThinnerEvent> events)
		{
			foreach (ThinnerEvent e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(userID))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<ThinEvent> GetCurrentEventAsync(Guid userID)
		{
			return events.FindAttendingEvent(userID);
		}


		private async Task ThrowIfUserAtEvent(Guid userID)
		{
			bool attendingEvent = false;
			try
			{
				// Throws an exception if there is no event
				await GetCurrentEventAsync(userID);
				attendingEvent = true;
			}
			catch { }

			if (attendingEvent)
			{ throw new InvalidUserException("User is currently attending an event."); }
		}
	}
}
