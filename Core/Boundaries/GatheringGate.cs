using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public enum GatheringState
	{ Upcoming, Open, Sealed, Ended }

    public enum GatheringBond
    { Surveying, Guest, Arrived, Left, Kicked }

    public record CoreGathering(ulong Id, UserShard Host, string Name, string Description,
		DateTimeOffset StartTime, double Latitude, double Longitude, string FriendlyLocation,
		DateTimeOffset? TimeEnded, GatheringState State, int GroupMinimum, int GroupMaximum, Character Character,
		double Radius, bool IsDynamic, bool IsPendingDeletion, int NumberOfGuests)
		: CoreOnlyData();

	public record GatheringShard(ulong Id, UserShard Host, string Name, string Description,
        DateTimeOffset StartTime, double Latitude, double Longitude, string FriendlyLocation,
		DateTimeOffset? TimeEnded, GatheringState State, int GroupMinimum, int GroupMaximum,
        double Radius, int NumberOfGuests, float RelativeAngle);

	public record GuestListShard(int Surveyers, int GuestCount,
		List<GuestListBondPair> Guests);

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
			int groupMinimum, int groupMaximum, Character character,
			double Radius, bool isDynamic);
		Task UpdateGatheringAsync(ulong gatheringId, List<(string Property, object Value)> edits);
		Task EndGatheringAsync(ulong gatheringId, DateTimeOffset time);
		Task DeleteGatheringAsync(ulong gatheringId);

		Task<GatheringBond?> GetUserStateAsync(ulong userId, ulong gatheringId);
		Task SetUserStateAsync(ulong userId, ulong gatheringId, GatheringBond userState, DateTimeOffset time);
		Task RemoveUserAsync(ulong userId, ulong gatheringId);

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
			string gatheringDescription = "", bool? isOpen = null,
			DateTimeOffset? startTime = null, double? latitude = null, double? longitude = null, string friendlyLocation = "",
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null);
		Task StartGatheringAsync(ulong userId, ulong gatheringId);
		Task EndGatheringAsync(ulong userId, ulong gatheringId);
		Task DeleteGatheringAsync(ulong userId, ulong gatheringId);

		Task SurveyGatheringAsync(ulong userId, ulong gatheringId);
		Task UnsurveyGatheringAsync(ulong userId, ulong gatheringId);
		Task JoinGatheringAsync(ulong userId, ulong gatheringId);
		Task LeaveGatheringAsync(ulong userId, ulong gatheringId);

		Task<GuestListShard> GetGuestListAsync(ulong userId, ulong gatheringId);
		Task<List<UserShard>> GetPotentialInviteesAsync(ulong userId, ulong gatheringId);
		Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong gatheringId);
		Task KickUserAsync(ulong hostId, ulong targetId, ulong gatheringId);
	}

	#endregion
}
