using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Server.Controls;
using Shared;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string Description, string EventType,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		bool IsOpen, int GroupMinimum, int GroupMaximum);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, string EventType, double Latitude, double Longitude);

	public record EventReport(Guid Id, Guid ReportingUserId, Guid ReportedEventId, Guid ReportedEventHostId, DateTimeOffset ReportTime,
		EventReportType ReportType, string ReportDetails);

	public interface IEventDatabase
	{
        public static IEventDatabase EventDatabaseAccess;
        ThinEvent FindEvent(Guid id);
		List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance);
		ThinEvent FindCurrentEvent(Guid id);
		List<ThinEvent> FindUpcomingEvents(Guid id);
		List<ThinEvent> FindPastEvents(Guid id);

		ThinEvent CreateEvent(Guid hostId, string name, string description, string eventType,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum);
		bool UpdateDescription(Guid id, string description);
		bool UpdateType(Guid id, string type);
		bool UpdateStatus(Guid id, bool isOpen);
		bool EndEvent(Guid id);

		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);

		List<ThinnerUser> GetGuestList(Guid id);

		List<EventReport> GetReportsAboutEvent(Guid id);
		bool ReportEvent(Guid userId, Guid eventId, EventReportType reportType, string reportDetails);
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
			string eventName, string eventDescription, string eventType,
			DateTimeOffset startTime, double latitude, double longitude,
			int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(Guid userID, Guid eventID,
			string eventDescription = "", string eventType = "",
			bool? isOpen = null);
		Task JoinEventAsync(Guid userID, Guid eventID);
		Task LeaveEventAsync(Guid userID, Guid eventID);
		Task EndEventAsync(Guid userID, Guid eventID);

		Task<List<ThinnerUser>> GetAttendeesAsync(Guid userID, Guid eventID);

		Task ReportEventAsync(Guid userID, Guid eventID, Guid HostId, EventReportType reportType, string reportDetails);
	}
}
