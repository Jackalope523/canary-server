using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Core.Boundaries
{
	public record EventShard(Guid Id, UserSilhouette Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		bool IsOpen, int GroupMinimum, int GroupMaximum, Character Character);
	public record EventThinSlice(Guid Id, UserSilhouette Host, double Latitude, double Longitude);

	public interface IEventDatabase
	{
        Task<EventShard> FindEventAsync(Guid id);
		Task<List<EventThinSlice>> FindEventsAsync(double latitude, double longitude, double distance);
		Task<EventShard> FindCurrentEventForUserAsync(Guid id);
		Task<List<EventShard>> FindUpcomingEventsForUserAsync(Guid id);
		Task<List<EventShard>> FindPastEventsForUserAsync(Guid id);

		Task<EventShard> CreateEventAsync(Guid hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character);
		Task<bool> UpdateEventAsync(Guid id, List<(string Property, object Value)> edits);
        Task<bool> EndEventAsync(Guid id);

        Task<bool> AddUserToEventAsync(Guid userId, Guid eventId);
        Task<bool> RemoveUserFromEventAsync(Guid userId, Guid eventId);

		Task<List<UserSilhouette>> GetGuestListAsync(Guid id);
		Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetGuestHistoryAsync(Guid id);
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
