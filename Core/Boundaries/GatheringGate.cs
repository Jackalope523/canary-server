using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public enum GatheringState
	{ Upcoming, OngoingOpen, OngoingHidden, Ended }

    public enum GatheringBond
    { Watching, Guest, Arrived, Left, Kicked }

    public record CoreGathering(ulong Id, UserShard Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, string FriendlyLocation,
		DateTimeOffset? TimeEnded, GatheringState State, int GroupMinimum, int GroupMaximum, CharacterShard Character,
		double Radius, bool IsDynamic, bool IsPendingDeletion, int NumberOfGuests)
		: CoreOnlyData();

	public record GatheringShard(ulong Id, UserShard Host, string Name, string Description,
        DateTimeOffset StartTime, double Latitude, double Longitude, string FriendlyLocation,
		DateTimeOffset? TimeEnded, GatheringState State, int GroupMinimum, int GroupMaximum,
        double Radius, int NumberOfGuests, float RelativeAngle);

	public record GuestListBondPair(UserShard User, GatheringBond Bond);

    #endregion

    #region Gates

    public interface IGatheringDatabase
	{
        Task<CoreGathering> FindGatheringAsync(ulong gatheringId);
		Task<List<CoreGathering>> FindGatheringsAsync(double latitude, double longitude, double distance);
		Task<CoreGathering> FindCurrentGatheringForUserAsync(ulong userId);
		Task<List<CoreGathering>> FindUpcomingGatheringsForUserAsync(ulong userId);
		Task<List<CoreGathering>> FindSurveyingGatheringsForUserAsync(ulong userId);
		Task<List<CoreGathering>> FindPastGatheringsForUserAsync(ulong userId);
		Task<List<CoreGathering>> FindGatheringsByUserAsync(ulong userId);

		Task<CoreGathering> CreateGatheringAsync(ulong hostId, string name, string description,
			DateTimeOffset startTime, double latitude, double longitude, string friendlyLocation,
			int groupMinimum, int groupMaximum, CharacterShard character,
			double Radius, bool isDynamic);
		Task UpdateGatheringAsync(ulong gatheringId, List<(string Property, object Value)> edits);
		Task TerminateGatheringAsync(ulong gatheringId, DateTimeOffset time);
		Task DeleteGatheringAsync(ulong gatheringId);

		Task<GatheringBond?> GetUserStateAsync(ulong userId, ulong gatheringId);
		Task SetUserStateAsync(ulong userId, ulong gatheringId, GatheringBond userState, DateTimeOffset time);
		Task DeleteUserStateAsync(ulong userId, ulong gatheringId);

		Task<List<(UserShard User, GatheringBond State)>> GetAllUsersAsync(ulong gatheringId);
		Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserShard User)>> GetGuestHistoryAsync(ulong gatheringId);
	}

	public interface IGatheringOperations
	{
		Task<GatheringShard> GetGatheringInformationAsync(ulong userId, ulong gatheringId);
		Task<List<GatheringShard>> GetGatheringsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance);
		Task<List<GatheringShard>> GetPersonalisedGatheringsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance);

		Task<GatheringShard> CreateGatheringAsync(ulong userId, string gatheringName, string gatheringDescription,
			DateTimeOffset startTime, double latitude, double longitude, string friendlyLocation,
			double radius, bool isDynamic, int? groupMinimum, int? groupMaximum,
			MemoryStream heroImage);
		Task EditGatheringAsync(ulong userId, ulong gatheringId,
			string gatheringName = "", string gatheringDescription = "",
			DateTimeOffset? startTime = null, double? latitude = null, double? longitude = null, string friendlyLocation = "",
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null,
			MemoryStream heroImage = null);
		Task StartGatheringAsync(ulong userId, ulong gatheringId);
		Task TerminateGatheringAsync(ulong userId, ulong gatheringId);
		Task DeleteGatheringAsync(ulong userId, ulong gatheringId);

		Task ChangeGatheringVisibilityAsync(ulong userId, ulong gatheringId, bool hide);

		Task WatchGatheringAsync(ulong userId, ulong gatheringId);
		Task UnwatchGatheringAsync(ulong userId, ulong gatheringId);
		Task JoinGatheringAsync(ulong userId, ulong gatheringId);
		Task CheckInToGatheringAsync(ulong userId, double latitude, double longitude);
		Task LeaveGatheringAsync(ulong userId, ulong gatheringId);

		Task<List<GuestListBondPair>> GetGuestListAsync(ulong userId, ulong gatheringId);
		Task<List<UserShard>> GetPotentialInviteesAsync(ulong userId, ulong gatheringId);
		Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong gatheringId);
		Task KickUserAsync(ulong hostId, ulong targetId, ulong gatheringId);

		Task<bool> AuthorisedToStart(ulong userId, ulong gatheringId);
		Task<bool> AuthorisedToJoin(ulong userId, ulong gatheringId);
		Task<bool> AuthorisedToUpload(ulong userId, ulong gatheringId);
	}

	#endregion
}
