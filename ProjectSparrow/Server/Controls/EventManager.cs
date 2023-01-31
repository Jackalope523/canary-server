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
			// TODO Verify user

			var eventInformation = events.FindEvent(eventID);

			return eventInformation;
		}

		public async Task<List<ThinnerEvent>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			// TODO Verify user
			
			List<ThinnerEvent> nearbyEvents = events.FindEvents(latitude, longitude, distance);

			return nearbyEvents;
		}

		public async Task<List<ThinnerEvent>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance)
		{
			// TODO Verify user

			List<ThinnerEvent> nearbyEvents = await GetEventsInAreaAsync(userID, latitude, longitude, distance);

			// TODO User interest weighting here

			return nearbyEvents;
		}

		public async Task<ThinEvent> CreateEventAsync(Guid userID,
			string eventName, string eventType, DateTime startTime,
			double latitude, double longitude)
		{
			// TODO Verify user

			// TODO Verify user is not currently at an event

			events.CreateEvent(userID, eventName, eventType, startTime, latitude, longitude);

			return null;
		}

		public async Task JoinEventAsync(Guid userID, Guid eventID)
		{
			// TODO Verify user

			bool success = events.AddUserToEvent(userID, eventID);

			if (!success)
			{ throw new UnexpectedFailureException("Could not join event."); }
		}

		public async Task LeaveEventAsync(Guid userID, Guid eventID)
		{
			// TODO Verify user

			bool success = events.RemoveUserFromEvent(userID, eventID);

            if (!success)
            { throw new UnexpectedFailureException("Could not leave event."); }
        }

		public async Task EndEventAsync(Guid userID, Guid eventID)
		{
			// TODO Verify user

			// TODO Verify user is event host

			bool success = events.EndEvent(eventID);

            if (!success)
            { throw new UnexpectedFailureException("Could not end event."); }
        }
		
		public async Task<List<ThinnerUser>> GetAttendeesAsync(Guid userID, Guid eventID)
		{
			// TODO Verify user

			// TODO Verify user is event host

			return events.GetGuestList(eventID);
		}
	}
}
