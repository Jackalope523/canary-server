using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Core.Controls;
using Shared;

namespace Core.Boundaries
{
	public record EventShard(ulong Id, UserSilhouette Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		bool IsOpen, int GroupMinimum, int GroupMaximum, Character Character);
	public record EventThinSlice(ulong Id, UserSilhouette Host, double Latitude, double Longitude);

	public interface IEventDatabase
	{
        EventShard FindEvent(ulong id);
		List<EventThinSlice> FindEvents(double latitude, double longitude, double distance);
		EventShard FindCurrentEventForUser(ulong id);
		List<EventShard> FindUpcomingEventsForUser(ulong id);
		List<EventShard> FindPastEventsForUser(ulong id);

		EventShard CreateEvent(ulong hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character);
		bool UpdateEvent(ulong id, List<(string Property, object Value)> edits);
		bool EndEvent(ulong id);

		EventUserState? GetUserState(ulong userId, ulong eventId);
		bool SetUserState(ulong userId, ulong eventId, EventUserState userState);
		bool RemoveUser(ulong userId, ulong eventId);

		List<UserSilhouette> GetWatchers(ulong id);
		List<UserSilhouette> GetAttendees(ulong id);
		List<UserSilhouette> GetGuests(ulong id);
		List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> GetGuestHistory(ulong id);
	}

	public interface IEventOperations
	{
		Task<EventShard> GetEventInformationAsync(ulong userID, ulong eventID);
		Task<List<EventThinSlice>> GetEventsInAreaAsync(ulong userID,
			double latitude, double longitude, double distance);
		Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(ulong userID,
			double latitude, double longitude, double distance);

		Task<EventShard> CreateEventAsync(ulong userID, string eventName, string eventDescription,
			DateTimeOffset startTime, double latitude, double longitude,
			int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(ulong userID, ulong eventID,
			string eventDescription = "", bool? isOpen = null);
		Task WatchEventAsync(ulong userID, ulong eventID);
		Task UnwatchEventAsync(ulong userID, ulong eventID);
		Task JoinEventAsync(ulong userID, ulong eventID);
		Task LeaveEventAsync(ulong userID, ulong eventID);
		Task EndEventAsync(ulong userID, ulong eventID);

		Task<List<UserSilhouette>> GetAttendeesAsync(ulong userID, ulong eventID);
	}
}
