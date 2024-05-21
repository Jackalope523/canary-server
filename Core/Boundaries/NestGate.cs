using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public enum UserRating
    { Positive, Negative, Remove }

    public record UserProfile(ulong Id, string Name, int Reputation, int NumberOfAppreciateers);
    public record UserSilhouette(ulong Id, string Name);
    public record NestShard(List<GatheringShard> Gatherings, List<SnapshotShard> Snapshots);
    public record AgendaShard(List<(GatheringShard Gathering, GatheringBond Bond)> Agenda);

	#endregion

	#region Gates

	public interface INestDatabase
    {
        Task<List<UserSilhouette>> GetCompanionsAsync(ulong userId);
		Task<List<UserSilhouette>> GetAppreciatedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetUsersAppreciatingAsync(ulong userId);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userId);

        Task AppreciateUserAsync(ulong userId, ulong targetUserId, DateTimeOffset time);
		Task UnappreciateUserAsync(ulong userId, ulong targetUserId);
		Task BlockUserAsync(ulong userId, ulong targetUserId, DateTimeOffset time);
		Task UnblockUserAsync(ulong userId, ulong targetUserId);

		Task RateUserAsync(ulong userId, ulong targetUserId, UserRating rating, DateTimeOffset time);
		Task RemoveUserRatingAsync(ulong userId, ulong targetUserId);
		Task<(int Positive, int Negative)> GetUserRatingsAsync(ulong userId);
    }

	public interface INestOperations
    {
        Task<NestShard> GetUserNestAsync(ulong userId, ulong targetId);

        Task<AgendaShard> GetUserAgendaAsync(ulong userId, ulong targetId);
        Task<IDictionary<UserSilhouette, AgendaShard>> GetCompanionAgendaAsync(ulong userId);

        Task<List<UserSilhouette>> GetCompanionsAsync(ulong userId);
        Task<List<UserSilhouette>> GetAppreciatedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId);

        Task AppreciateUserAsync(ulong userId, ulong targetId);
        Task UnappreciateUserAsync(ulong userId, ulong targetId);
        Task BlockUserAsync(ulong userId, ulong targetId);
        Task UnblockUserAsync(ulong userId, ulong targetId);
    }

	#endregion
}

