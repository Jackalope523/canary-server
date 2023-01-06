using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IEventOperations
	{
		(string hostID, float latitude, float longitude, string eventType, DateTime startTime) GetEventInformation(string identification, string eventID);
		List<(string eventID, float latitude, float longitude, string eventType)> GetEventsInArea(string identification, float latitude, float longitude, float distance);
		List<(string eventID, float latitude, float longitude, string eventType)> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance);

		void CreateEvent(string identification, float latitude, float longitude);
		void JoinEvent(string identification, string eventID);
		void LeaveEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<(string userID, string name, string profilePhoto)> GetAttendees(string identification, string eventID);
	}
}
