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
        EventShard FindEvent(ulong eventId);
		List<EventThinSlice> FindEvents(double latitude, double longitude, double distance);
		EventShard FindCurrentEventForUser(ulong userId);
		List<EventShard> FindUpcomingEventsForUser(ulong userId);
		List<EventShard> FindPastEventsForUser(ulong userId);

		EventShard CreateEvent(ulong hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character);
		bool UpdateEvent(ulong eventId, List<(string Property, object Value)> edits);
		bool EndEvent(ulong eventId);

		EventUserState? GetUserState(ulong userId, ulong eventId);
		bool SetUserState(ulong userId, ulong eventId, EventUserState userState);
		bool RemoveUser(ulong userId, ulong eventId);

		List<UserSilhouette> GetWatchers(ulong eventId);
		List<UserSilhouette> GetAttendees(ulong eventId);
		List<UserSilhouette> GetGuests(ulong eventId);
		List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> GetGuestHistory(ulong eventId);
	}

	public interface IEventOperations
	{
		Task<EventShard> GetEventInformationAsync(ulong userId, ulong eventId);
		Task<List<EventThinSlice>> GetEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance);
		Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance);

		Task<EventShard> CreateEventAsync(ulong userId, string eventName, string eventDescription,
			DateTimeOffset startTime, double latitude, double longitude,
			int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(ulong userId, ulong eventId,
			string eventDescription = "", bool? isOpen = null);
		Task WatchEventAsync(ulong userId, ulong eventId);
		Task UnwatchEventAsync(ulong userId, ulong eventId);
		Task JoinEventAsync(ulong userId, ulong eventId);
		Task LeaveEventAsync(ulong userId, ulong eventId);
		Task EndEventAsync(ulong userId, ulong eventId);

		Task<List<UserSilhouette>> GetAttendeesAsync(ulong userId, ulong eventId);
	}
}
