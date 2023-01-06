using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IEventOperations
	{
		void GetEventInformation(string identification, string eventID);
		List<string> GetEventsInArea(string identification, float latitude, float longitude, float distance);
		List<string> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance);

		void CreateEvent(string identification, float latitude, float longitude);
		void JoinEvent(string identification, string eventID);
		void LeaveEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<string> GetAttendees(string identification, string eventID);
	}
}
