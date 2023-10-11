using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Server.Controls;
using Shared;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, DateTimeOffset? TimeEnded,
		bool IsOpen, int GroupMinimum, int GroupMaximum, Character Character);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, double Latitude, double Longitude);
	public record EventHeader(Guid Id, string Name, bool IsActive, DateTimeOffset LastActiveTime);

	public record EventReport(Guid Id, Guid ReportingUserId, Guid ReportedEventId, Guid ReportedEventHostId, DateTimeOffset ReportTime,
		EventReportType ReportType, string ReportDetails);

	public record EventPost(Guid Id, Guid EventId, Guid UserId, DateTimeOffset TimePosted, string ImageURL, (int Positive, int Negative) Ratings);

	public interface IEventDatabase
	{
        public static IEventDatabase EventDatabaseAccess;

        ThinEvent FindEvent(Guid id);
		List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance);
		ThinEvent FindCurrentEvent(Guid id);
		List<ThinEvent> FindUpcomingEvents(Guid id);
		List<ThinEvent> FindPastEvents(Guid id);

		ThinEvent CreateEvent(Guid hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude,
			int groupMinimum, int groupMaximum, Character character);
		bool UpdateDescription(Guid id, string description);
		bool UpdateStatus(Guid id, bool isOpen);
		bool EndEvent(Guid id);

		bool AddUserToEvent(Guid userId, Guid eventId);
		bool RemoveUserFromEvent(Guid userId, Guid eventId);

		List<ThinnerUser> GetGuestList(Guid id);
		List<(DateTimeOffset Joined, DateTimeOffset Left, ThinnerUser User)> GetGuestHistory(Guid id);

		List<EventReport> GetReportsAboutEvent(Guid id);
		bool ReportEvent(Guid userId, Guid eventId, Guid HostId,
			EventReportType reportType, string reportDetails);

		List<EventPost> GetPostsForEvent(Guid id);
		List<EventPost> GetPostsForUser(Guid id);
		EventPost GetPost(Guid id);
		EventPost AddPost(Guid eventId, Guid posterId,
			DateTimeOffset timePosted, string imageURL);
		bool RemovePost(Guid postId);
		bool RatePost(Guid postId, Guid voterId, UserRating rating);
		bool RemovePostRating(Guid postId, Guid voterId);

		List<EventPost> GenerateFeedForUser(Guid id, DateTimeOffset depthCharge, List<Guid> exclusionList);
	}

	public interface IEventOperations
	{
		static IEventOperations EventManager => new EventManager(IAccountDatabase.AccountDatabaseAccess, IEventDatabase.EventDatabaseAccess);

		Task<ThinEvent> GetEventInformationAsync(Guid userID, Guid eventID);
		Task<List<ThinnerEvent>> GetEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance);
		Task<List<ThinnerEvent>> GetPersonalisedEventsInAreaAsync(Guid userID,
			double latitude, double longitude, double distance);

		Task<ThinEvent> CreateEventAsync(Guid userID, string eventName, string eventDescription,
			DateTimeOffset startTime, double latitude, double longitude,
			int? groupMinimum, int? groupMaximum);
		Task EditEventAsync(Guid userID, Guid eventID,
			string eventDescription = "", bool? isOpen = null);
		Task JoinEventAsync(Guid userID, Guid eventID);
		Task LeaveEventAsync(Guid userID, Guid eventID);
		Task EndEventAsync(Guid userID, Guid eventID);

		Task<List<ThinnerUser>> GetAttendeesAsync(Guid userID, Guid eventID);

		Task ReportEventAsync(Guid userID, Guid eventID, Guid hostId,
			EventReportType reportType, string reportDetails);

		Task<List<EventPost>> GetEventPostsAsync(Guid userID, Guid eventID);
		Task<EventPost> AddPostAsync(Guid userID, Guid eventID, string imageURL);
		Task RemovePostAsync(Guid userID, Guid postID);
		Task RatePostAsync(Guid userID, Guid postID, UserRating rating);

		Task<(int Depth, List<EventHeader> Headers, List<EventPost> Posts)> GetUserFeedAsync(Guid userID,
			int depth, List<Guid> exclusionList = null);
	}
}
