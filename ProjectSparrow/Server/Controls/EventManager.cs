using Server.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controls
{
	public class EventManager : IEventOperations
	{
		public void GetEventInformation(string identification, string eventID)
		{
			throw new NotImplementedException();
		}

		public List<string> GetEventsInArea(string identification, float latitude, float longitude, float distance)
		{
			throw new NotImplementedException();
		}

		public List<string> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance)
		{
			throw new NotImplementedException();
		}

		public void CreateEvent(string identification, float latitude, float longitude)
		{
			throw new NotImplementedException();
		}

		public void JoinEvent(string identification, string eventID)
		{
			throw new NotImplementedException();
		}

		public void LeaveEvent(string identification)
		{
			throw new NotImplementedException();
		}

		public void EndEvent(string identification)
		{
			throw new NotImplementedException();
		}

		public List<string> GetAttendees(string identification, string eventID)
		{
			throw new NotImplementedException();
		}
	}
}
