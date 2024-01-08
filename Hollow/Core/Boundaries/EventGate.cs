using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Core.Controls;
using Shared;

namespace Core.Boundaries
{
	#region Schemas

	public enum EventState
	{ Upcoming, Open, Sealed, Ended }

	public record EventShard(ulong Id, UserSilhouette Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		EventState State, int GroupMinimum, int GroupMaximum, Character Character,
		double Radius, bool IsDynamic);
	public record EventThinSlice(ulong Id, UserSilhouette Host, double Latitude, double Longitude);

	#endregion

	#region Gates

	public interface IEventDatabase
	{
        Task<EventShard> FindEventAsync(ulong eventId);
		Task<List<EventThinSlice>> FindEventsAsync(double latitude, double longitude, double distance);
		Task<EventShard> FindCurrentEventForUserAsync(ulong userId);
		Task<List<EventShard>> FindUpcomingEventsForUserAsync(ulong userId);
		Task<List<EventShard>> FindPastEventsForUserAsync(ulong userId);
		Task<List<EventShard>> FindEventsByUserAsync(ulong userId);

		Task<EventShard> CreateEventAsync(ulong hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character,
			double Radius, bool isDynamic);
		Task UpdateEventAsync(ulong eventId, List<(string Property, object Value)> edits);
		Task EndEventAsync(ulong eventId);

		Task<EventBond?> GetUserStateAsync(ulong userId, ulong eventId);
		Task SetUserStateAsync(ulong userId, ulong eventId, EventBond userState);
		Task RemoveUserAsync(ulong userId, ulong eventId);

		Task<List<(UserSilhouette User, EventBond State)>> GetAllUsersAsync(ulong eventId);
		Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetGuestHistoryAsync(ulong eventId);
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
			double radius, bool isDynamic, int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(ulong userId, ulong eventId,
			string eventDescription = "", bool? isOpen = null,
			DateTimeOffset? startTime = null, double? latitude = null, double? longitude = null,
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null);
		Task StartEventAsync(ulong userId, ulong eventId);
		Task EndEventAsync(ulong userId, ulong eventId);

		Task WatchEventAsync(ulong userId, ulong eventId);
		Task UnwatchEventAsync(ulong userId, ulong eventId);
		Task JoinEventAsync(ulong userId, ulong eventId);
		Task LeaveEventAsync(ulong userId, ulong eventId);

		Task<(int Watchers, int GuestCount, List<(UserSilhouette User, EventBond State)> Guests)>
			GetGuestListAsync(ulong userId, ulong eventId);
		Task<List<UserSilhouette>> GetPotentialInviteesAsync(ulong userId, ulong eventId);
		Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong eventId);
		Task KickUserAsync(ulong hostId, ulong targetId, ulong eventId);
	}

	#endregion
}
