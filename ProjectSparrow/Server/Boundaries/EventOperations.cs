using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IEventOperations
	{
		public void GetEventInformation(string identification, string eventID);
		public void GetEventsInArea(string identification, float latitude, float longitude, float distance);
		public void GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance);

		public void CreateEvent(string identification, float latitude, float longitude);
		public void JoinEvent(string identification, string eventID);
		public void LeaveEvent(string identification);
		public void EndEvent(string identification);

		public void GetAttendees(string identification, string eventID);
	}
}
