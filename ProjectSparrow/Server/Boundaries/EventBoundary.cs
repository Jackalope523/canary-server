using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public record ThinEvent(string HostID, float EventName, string EventType, DateTime StartTime, float Latitude, float Longitude);
	public record ThinListEvent(string HostID, string EventType, float Latitude, float Longitude);

	public record ThinListUser(string UserID, string Name, string ProfilePhoto);

	public interface IEventDatabase
	{
		ThinEvent GetEvent(string eventID);
		List<ThinListEvent> GetEvents(float latitude, float longitude, float distance);

		void CreateEvent(string hostID, string eventName, string eventType, DateTime startTime, float latitude, float longitude);
		void AddUserToEvent(string identification, string eventID);
		void RemoveUserFromEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<ThinListUser> GetGuestList(string identification, string eventID);
	}

	public interface IEventOperations
	{
		ThinEvent GetEventInformation(string identification, string eventID);
		List<ThinListEvent> GetEventsInArea(string identification, float latitude, float longitude, float distance);
		List<ThinListEvent> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance);

		void CreateEvent(string identification, string eventName, string eventType, DateTime startTime, float latitude, float longitude);
		void JoinEvent(string identification, string eventID);
		void LeaveEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<ThinListUser> GetAttendees(string identification, string eventID);
	}
}
