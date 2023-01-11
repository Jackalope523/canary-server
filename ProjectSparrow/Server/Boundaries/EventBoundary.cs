using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string EventType, DateTime StartTime, double Latitude, double Longitude);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, string EventType, double Latitude, double Longitude);

	public interface IEventDatabase
	{
        public static IEventDatabase EventDatabaseAccess;
        ThinEvent FindEvent(Guid id);
		List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance);

		bool CreateEvent(Guid hostId, string name, string eventType, DateTime startTime, double latitude, double longitude);
		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);  
		bool EndEvent(Guid id);

		List<ThinnerUser> GetGuestList(Guid id);
	}

	public interface IEventOperations
	{
		ThinEvent GetEventInformation(Guid userID, Guid eventID);
		List<ThinnerEvent> GetEventsInArea(Guid userID, double latitude, double longitude, double distance);
		List<ThinnerEvent> GetPersonalisedEventsInArea(Guid userID, double latitude, double longitude, double distance);

		void CreateEvent(Guid userID, string eventName, string eventType, DateTime startTime, double latitude, double longitude);
		void JoinEvent(Guid userID, Guid eventID);
		void LeaveEvent(Guid userID, Guid eventID);
		void EndEvent(Guid userID, Guid eventID);

		List<ThinnerUser> GetAttendees(Guid userID, Guid eventID);
	}
}
