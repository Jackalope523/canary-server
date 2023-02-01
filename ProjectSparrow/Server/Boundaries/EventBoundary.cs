using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Server.Controls;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string EventType, DateTime StartTime, double Latitude, double Longitude);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, string EventType, double Latitude, double Longitude);

	public interface IEventDatabase
	{
        public static IEventDatabase EventDatabaseAccess;
        ThinEvent FindEvent(Guid id);
		List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance);
		ThinEvent FindAttendingEvent(Guid id);

		ThinEvent CreateEvent(Guid hostId, string name, string eventType, DateTime startTime, double latitude, double longitude);
		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);  
		bool EndEvent(Guid id);

		List<ThinnerUser> GetGuestList(Guid id);
	}

	public interface IEventOperations
	{
		static IEventOperations EventManager => new EventManager(IAccountDatabase.AccountDatabaseAccess, IEventDatabase.EventDatabaseAccess);

		Task<ThinEvent> GetEventInformationAsync(Guid userID, Guid eventID);
		Task<List<ThinnerEvent>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance);
		Task<List<ThinnerEvent>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance);

		Task<ThinEvent> CreateEventAsync(Guid userID,
			string eventName, string eventType, DateTime startTime,
			double latitude, double longitude);
		Task JoinEventAsync(Guid userID, Guid eventID);
		Task LeaveEventAsync(Guid userID, Guid eventID);
		Task EndEventAsync(Guid userID, Guid eventID);

		Task<List<ThinnerUser>> GetAttendeesAsync(Guid userID, Guid eventID);
	}
}
