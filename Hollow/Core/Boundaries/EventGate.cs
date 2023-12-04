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
	{ upcoming, active_open, active_closed, ended }

	public record EventShard(ulong Id, UserSilhouette Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		EventState State, int GroupMinimum, int GroupMaximum, Character Character,
		double Radius, bool IsDynamic);
	public record EventThinSlice(ulong Id, UserSilhouette Host, double Latitude, double Longitude);

	#endregion

	#region Gates

	public interface IEventDatabase
	{
        EventShard FindEvent(ulong eventId);
		List<EventThinSlice> FindEvents(double latitude, double longitude, double distance);
		EventShard FindCurrentEventForUser(ulong userId);
		List<EventShard> FindUpcomingEventsForUser(ulong userId);
		List<EventShard> FindPastEventsForUser(ulong userId);
		List<EventShard> FindEventsByUser(ulong userId);

		EventShard CreateEvent(ulong hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character,
			double Radius, bool isDynamic);
		bool UpdateEvent(ulong eventId, List<(string Property, object Value)> edits);
		bool EndEvent(ulong eventId);

		EventUserState? GetUserState(ulong userId, ulong eventId);
		bool SetUserState(ulong userId, ulong eventId, EventUserState userState);
		bool RemoveUser(ulong userId, ulong eventId);

		List<(UserSilhouette User, EventUserState State)> GetAllUsers(ulong eventId);
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

		Task<(int Watchers, int GuestCount, List<(UserSilhouette User, EventUserState State)> Guests)>
			GetGuestListAsync(ulong userId, ulong eventId);
		Task<List<UserSilhouette>> GetPotentialInviteesAsync(ulong userId, ulong eventId);
		Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong eventId);
		Task KickUserAsync(ulong hostId, ulong targetId, ulong eventId);
	}

	#endregion
}
