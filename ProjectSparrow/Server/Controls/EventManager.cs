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
		private IEventDatabase events;

		public EventManager(IEventDatabase eventDatabase)
		{
			events = eventDatabase;
		}

		public ThinEvent GetEventInformation(string identification, string eventID)
		{
			var eventInformation = events.GetEvent(eventID);

			return eventInformation;
		}

		public List<ThinListEvent> GetEventsInArea(string identification, float latitude, float longitude, float distance)
		{
			List<ThinListEvent> nearbyEvents = events.GetEvents(latitude, longitude);

			// Distance calculations here

			return nearbyEvents;
		}

		public List<ThinListEvent> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance)
		{
			List<ThinListEvent> nearbyEvents = GetEventsInArea(identification, latitude, longitude, distance);

			// User interest weighting here

			return nearbyEvents;
		}

		public void CreateEvent(string identification, float latitude, float longitude)
		{
			// Get hostID from identification and send it in

			events.CreateEvent("hostID", latitude, longitude);
		}

		public void JoinEvent(string identification, string eventID)
		{
			events.JoinEvent(identification, eventID);
		}

		public void LeaveEvent(string identification, string eventID)
		{
			events.LeaveEvent(identification, eventID);
		}

		public void EndEvent(string identification, string eventID)
		{
			events.EndEvent(identification, eventID);
		}

		public List<ThinListUser> GetAttendees(string identification, string eventID)
		{
			return events.GetGuestList(identification, eventID); // Do we do user verification on server or DB? Maybe change DB boundary verbs to things like 'AccountCreated()'
		}
	}
}
