using Server.Boundaries;
using Server.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Server.Controls
{
	public class EventManager : IEventOperations
	{
		private IAccountDatabase accounts;
		private IEventDatabase events;

		public EventManager(IAccountDatabase accountDatabase, IEventDatabase eventDatabase)
		{
			accounts = accountDatabase;
			events = eventDatabase;
		}

		public async Task<ThinEvent> GetEventInformationAsync(Guid userID, Guid eventID)
        {
			// Check if user is allowed to view event
			if (!await UserCanSeeEvent(userID, eventID))
			{
				throw new InvalidEventException("User is unable to view event.");
			}

			var eventInformation = events.FindEvent(eventID);
			return eventInformation;
		}

		public async Task<List<ThinnerEvent>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			var nearbyEvents = events.FindEvents(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			foreach (ThinnerEvent e in nearbyEvents)
			{
				if (!await UserCanSeeEvent(userID, e.Id))
				{
					nearbyEvents.Remove(e);
				}
			}

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
			string eventName, string eventType, DateTime startTime,
			double latitude, double longitude)
		{
			try
			{
				// Will throw if the user is not attending an event
				events.FindAttendingEvent(userID);
			}
			catch
			{
				// TODO Validate event
				// Try to create an event
				var newEvent = events.CreateEvent(userID, eventName, eventType, startTime, latitude, longitude);
				return newEvent;
			}

			throw new InvalidUserException("User cannot create a new event whilst attending one.");
		}

		public async Task JoinEventAsync(Guid userID, Guid eventID)
		{
			// Check if user is allowed to view event
			if (!await UserCanSeeEvent(userID, eventID))
			{
				throw new InvalidEventException("User is unable to view event.");
			}

			// Try to add user to the event
			bool success = events.AddUserToEvent(userID, eventID);
			if (!success)
			{ throw new UnexpectedFailureException("Could not join event."); }
		}

		public async Task LeaveEventAsync(Guid userID, Guid eventID)
		{
			// Try to remove user from event
			bool success = events.RemoveUserFromEvent(userID, eventID);
            if (!success)
            { throw new UnexpectedFailureException("Could not leave event."); }
        }

		public async Task EndEventAsync(Guid userID, Guid eventID)
		{
			// Check if the user is able to end the event
			if (await UserIsEventHost(userID, eventID))
			{
				throw new InvalidUserException("User does not have permissions to end event.");
			}

			// Try to end to event
			bool success = events.EndEvent(eventID);
            if (!success)
            { throw new UnexpectedFailureException("Could not end event."); }
        }
		
		public async Task<List<ThinnerUser>> GetAttendeesAsync(Guid userID, Guid eventID)
		{
			var guestList = events.GetGuestList(eventID);

			// Check if user is on the guest list
			if (guestList.Find(x => x.Id == userID) == null)
			{
				throw new InvalidUserException("User did not attend event.");
			}

			return guestList;
		}

		private async Task<bool> UserCanSeeEvent(Guid userID, Guid eventID)
		{
			var host = events.FindEvent(eventID);
			var hostBlockedList = accounts.GetBlockedUsers(userID);

			// Check if user is blocked by event host
			if (hostBlockedList.Find(x => x.Id == userID) != null)
			{
				return false;
			}

			return true;
		}

		private async Task<bool> UserIsEventHost(Guid userID, Guid eventID)
		{
			var @event = events.FindEvent(eventID);
			return @event.Host.Id == userID;
		}
	}
}
