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

		public ThinEvent GetEventInformation(string identification, string eventID)
		{
			var eventInformation = events.GetEvent(eventID);

			return eventInformation;
		}

		public List<ThinListEvent> GetEventsInArea(string identification, float latitude, float longitude, float distance)
		{
			List<ThinListEvent> nearbyEvents = events.GetEvents(latitude, longitude, distance);

			// TODO Distance calculations here

			return nearbyEvents;
		}

		public List<ThinListEvent> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance)
		{
			List<ThinListEvent> nearbyEvents = GetEventsInArea(identification, latitude, longitude, distance);

			// TODO User interest weighting here

			return nearbyEvents;
		}

		public void CreateEvent(string identification, string eventName, string eventType, DateTime startTime, float latitude, float longitude)
		{
			accounts.FindAccount(identification);

			events.CreateEvent("hostID", eventName, eventType, startTime, latitude, longitude);
		}

		public void JoinEvent(string identification, string eventID)
		{
			events.AddUserToEvent(identification, eventID);
		}

		public void LeaveEvent(string identification, string eventID)
		{
			events.RemoveUserFromEvent(identification, eventID);
		}

		public void EndEvent(string identification, string eventID)
		{
			events.EndEvent(identification, eventID);
		}

		public List<ThinListUser> GetAttendees(string identification, string eventID)
		{
			return events.GetGuestList(identification, eventID);
		}
	}
}
