using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public enum UserRating
    { Positive, Negative, Remove }

    public record UserProfile(ulong Id, string Name, int Reputation, int NumberOfFollowers);
    public record UserSilhouette(ulong Id, string Name);

	#endregion

	#region Gates

	public interface IProfileDatabase
    {
        Task<List<UserSilhouette>> GetFriendsAsync(ulong userId);
		Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetUsersFollowingAsync(ulong userId);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userId);

        Task FollowUserAsync(ulong userId, ulong targetUserId, DateTimeOffset time);
		Task UnfollowUserAsync(ulong userId, ulong targetUserId);
		Task BlockUserAsync(ulong userId, ulong targetUserId, DateTimeOffset time);
		Task UnblockUserAsync(ulong userId, ulong targetUserId);

		Task RateUserAsync(ulong userId, ulong targetUserId, UserRating rating, DateTimeOffset time);
		Task RemoveUserRatingAsync(ulong userId, ulong targetUserId);
		Task<(int Positive, int Negative)> GetUserRatingsAsync(ulong userId);
    }

	public interface IProfileOperations
    {
        Task<UserProfile> GetUserProfileAsync(ulong userId, ulong targetId);
        Task<(List<EventShard> Events, List<Etching> Etchings)> GetUserNestAsync(ulong userId, ulong targetId);

        Task<List<(EventShard, EventBond)>> GetUserActivityAsync(ulong userId, ulong targetId);
        Task<IDictionary<UserSilhouette, List<(EventShard, EventBond)>>> GetFriendActivityAsync(ulong userId);

        Task<List<UserSilhouette>> GetFriendsAsync(ulong userId);
        Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId);

        Task FollowUserAsync(ulong userId, ulong targetId);
        Task UnfollowUserAsync(ulong userId, ulong targetId);
        Task BlockUserAsync(ulong userId, ulong targetId);
        Task UnblockUserAsync(ulong userId, ulong targetId);

        Task RateUserAsync(ulong userId, ulong targetId, UserRating rating);
    }

	#endregion
}

