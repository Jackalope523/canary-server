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

		public ThinEvent GetEventInformation(Guid userID, Guid eventID)
        {
			// TODO Verify user

			var eventInformation = events.FindEvent(eventID);

			return eventInformation;
		}

		public List<ThinnerEvent> GetEventsInArea(Guid userID, float latitude, float longitude, float distance)
		{
			// TODO Verify user
			
			List<ThinnerEvent> nearbyEvents = events.FindEvents(latitude, longitude, distance);

			return nearbyEvents;
		}

		public List<ThinnerEvent> GetPersonalisedEventsInArea(Guid userID, float latitude, float longitude, float distance)
		{
			// TODO Verify user

			List<ThinnerEvent> nearbyEvents = GetEventsInArea(userID, latitude, longitude, distance);

			// TODO User interest weighting here

			return nearbyEvents;
		}

		public void CreateEvent(Guid userID, string eventName, string eventType, DateTime startTime, float latitude, float longitude)
		{
			// TODO Verify user

			// TODO Verify user is not currently at an event

			events.CreateEvent(userID, eventName, eventType, startTime, latitude, longitude);
		}

		public void JoinEvent(Guid userID, Guid eventID)
		{
			// TODO Verify user

			bool success = events.AddUserToEvent(userID, eventID);

			if (!success)
			{ throw new UnexpectedFailureException("Could not join event."); }
		}

		public void LeaveEvent(Guid userID, Guid eventID)
		{
			// TODO Verify user

			bool success = events.RemoveUserFromEvent(userID, eventID);

            if (!success)
            { throw new UnexpectedFailureException("Could not leave event."); }
        }

		public void EndEvent(Guid userID, Guid eventID)
		{
			// TODO Verify user

			// TODO Verify user is event host

			bool success = events.EndEvent(eventID);

            if (!success)
            { throw new UnexpectedFailureException("Could not end event."); }
        }
		
		public List<ThinnerUser> GetAttendees(Guid userID, Guid eventID)
		{
			// TODO Verify user

			// TODO Verify user is event host

			return events.GetGuestList(eventID);
		}
	}
}
