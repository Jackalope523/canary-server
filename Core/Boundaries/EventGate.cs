using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

	public enum EventState
	{ Upcoming, Open, Sealed, Ended }

    public enum EventBond
    { Watching, Guest, Arrived, Left, Kicked }

    public record CoreEvent(ulong Id, UserSilhouette Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		EventState State, int GroupMinimum, int GroupMaximum, Character Character,
		double Radius, bool IsDynamic, bool IsPendingDeletion, int NumberOfGuests)
		: CoreOnlyData();

	public record EventShard(ulong Id, UserSilhouette Host, string Name, string Description,
        DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
        EventState State, int GroupMinimum, int GroupMaximum,
        double Radius, int NumberOfGuests);

	public record GuestListShard(int Watchers, int GuestCount,
		List<(UserSilhouette User, EventBond Bond)> Guests);

    #endregion

    #region Gates

    public interface IEventDatabase
	{
        Task<CoreEvent> FindEventAsync(ulong eventId);
		Task<List<CoreEvent>> FindEventsAsync(double latitude, double longitude, double distance);
		Task<CoreEvent> FindCurrentEventForUserAsync(ulong userId);
		Task<List<CoreEvent>> FindUpcomingEventsForUserAsync(ulong userId);
		Task<List<CoreEvent>> FindWatchingEventsForUserAsync(ulong userId);
		Task<List<CoreEvent>> FindPastEventsForUserAsync(ulong userId);
		Task<List<CoreEvent>> FindEventsByUserAsync(ulong userId);

		Task<CoreEvent> CreateEventAsync(ulong hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character,
			double Radius, bool isDynamic);
		Task UpdateEventAsync(ulong eventId, List<(string Property, object Value)> edits);
		Task EndEventAsync(ulong eventId, DateTimeOffset time);
		Task DeleteEventAsync(ulong eventId);

		Task<EventBond?> GetUserStateAsync(ulong userId, ulong eventId);
		Task SetUserStateAsync(ulong userId, ulong eventId, EventBond userState, DateTimeOffset time);
		Task RemoveUserAsync(ulong userId, ulong eventId);

		Task<List<(UserSilhouette User, EventBond State)>> GetAllUsersAsync(ulong eventId);
		Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetGuestHistoryAsync(ulong eventId);
	}

	public interface IEventOperations
	{
		Task<EventShard> GetEventInformationAsync(ulong userId, ulong eventId);
		Task<List<EventShard>> GetEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance);
		Task<List<EventShard>> GetPersonalisedEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance);

		Task<EventShard> CreateEventAsync(ulong userId, string eventName, string eventDescription,
			DateTimeOffset startTime, double latitude, double longitude,
			double radius, bool isDynamic, int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(ulong userId, ulong eventId,
			string eventDescription = "", bool? isOpen = null,
			DateTimeOffset? startTime = null, double? latitude = null, double? longitude = null,
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null);
		Task StartEventAsync(ulong userId, ulong eventId);
		Task EndEventAsync(ulong userId, ulong eventId);
		Task DeleteEventAsync(ulong userId, ulong eventId);

		Task WatchEventAsync(ulong userId, ulong eventId);
		Task UnwatchEventAsync(ulong userId, ulong eventId);
		Task JoinEventAsync(ulong userId, ulong eventId);
		Task LeaveEventAsync(ulong userId, ulong eventId);

		Task<GuestListShard> GetGuestListAsync(ulong userId, ulong eventId);
		Task<List<UserSilhouette>> GetPotentialInviteesAsync(ulong userId, ulong eventId);
		Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong eventId);
		Task KickUserAsync(ulong hostId, ulong targetId, ulong eventId);
	}

	#endregion
}
