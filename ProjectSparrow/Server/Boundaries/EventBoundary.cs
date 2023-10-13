using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Server.Controls;
using Shared;

namespace Server.Boundaries
{
	public record EventShard(Guid Id, UserSilhouette Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		bool IsOpen, int GroupMinimum, int GroupMaximum, Character Character);
	public record EventThinSlice(Guid Id, UserSilhouette Host, double Latitude, double Longitude);

	public interface IEventDatabase
	{
        EventShard FindEvent(Guid id);
		List<EventThinSlice> FindEvents(double latitude, double longitude, double distance);
		EventShard FindCurrentEventForUser(Guid id);
		List<EventShard> FindUpcomingEventsForUser(Guid id);
		List<EventShard> FindPastEventsForUser(Guid id);

		EventShard CreateEvent(Guid hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character);
		bool UpdateEvent(Guid id, List<(string Property, object Value)> edits);
		bool EndEvent(Guid id);

		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);

		List<UserSilhouette> GetGuestList(Guid id);
		List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> GetGuestHistory(Guid id);
	}

	public interface IEventOperations
	{
		Task<EventShard> GetEventInformationAsync(Guid userID, Guid eventID);
		Task<List<EventThinSlice>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance);
		Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance);

		Task<EventShard> CreateEventAsync(Guid userID, string eventName, string eventDescription,
			DateTimeOffset startTime, double latitude, double longitude,
			int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(Guid userID, Guid eventID,
			string eventDescription = "", bool? isOpen = null);
		Task JoinEventAsync(Guid userID, Guid eventID);
		Task LeaveEventAsync(Guid userID, Guid eventID);
		Task EndEventAsync(Guid userID, Guid eventID);

		Task<List<UserSilhouette>> GetAttendeesAsync(Guid userID, Guid eventID);
	}
}
