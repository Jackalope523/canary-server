using System;
using System.Collections.Generic;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string EventType, DateTime StartTime, float Latitude, float Longitude);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, string EventType, float Latitude, float Longitude);

	public interface IEventDatabase
	{
		ThinEvent FindEvent(Guid Id);
		List<ThinnerEvent> FindEvents(float latitude, float longitude, float distance);

		bool CreateEvent(Guid hostID, string name, string eventType, DateTime startTime, float latitude, float longitude);
		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);  
		bool EndEvent(Guid Id);

		List<ThinnerUser> GetGuestList(Guid Id);
	}

	public interface IEventOperations
	{
		ThinEvent GetEventInformation(Guid userID, Guid eventID);
		List<ThinnerEvent> GetEventsInArea(Guid userID, float latitude, float longitude, float distance);
		List<ThinnerEvent> GetPersonalisedEventsInArea(Guid userID, float latitude, float longitude, float distance);

		void CreateEvent(Guid userID, string eventName, string eventType, DateTime startTime, float latitude, float longitude);
		void JoinEvent(Guid userID, Guid eventID);
		void LeaveEvent(Guid userID, Guid eventID);
		void EndEvent(Guid userID, Guid eventID);

		List<ThinnerUser> GetAttendees(Guid userID, Guid eventID);
	}
}
