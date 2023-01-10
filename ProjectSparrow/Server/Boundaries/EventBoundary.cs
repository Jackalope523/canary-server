using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string EventType, DateTime StartTime, float Latitude, float Longitude);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, string EventType, float Latitude, float Longitude);

	public interface IEventDatabase
	{
		ThinEvent FindEvent(Guid id);
		List<ThinnerEvent> FindEvents(float latitude, float longitude, float distance);

		bool CreateEvent(Guid hostId, string name, string eventType, DateTime startTime, float latitude, float longitude);
		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);  
		bool EndEvent(Guid Id);

		List<ThinnerUser> GetGuestList(Guid Id);
	}

	public interface IEventOperations
	{
		ThinEvent GetEventInformation(Guid UserID, string eventId);
		List<ThinnerEvent> GetEventsInArea(Guid UserID, float latitude, float longitude, float distance);
		List<ThinnerEvent> GetPersonalisedEventsInArea(Guid UserID, float latitude, float longitude, float distance);

		void CreateEvent(Guid UserID, string eventName, string eventType, DateTime startTime, float latitude, float longitude);
		void JoinEvent(Guid UserID, string eventId);
		void LeaveEvent(Guid UserID, string eventId);
		void EndEvent(Guid UserID, string eventId);

		List<ThinnerUser> GetAttendees(Guid UserID, string eventID);
	}
}
