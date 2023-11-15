using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    public record UserProfile(ulong Id, string Name, int Reputation, int NumberOfFollowers);
    public record UserSilhouette(ulong Id, string Name);

    public interface IProfileDatabase
    {
        List<UserSilhouette> GetFriends(ulong userId);
        List<UserSilhouette> GetFollowedUsers(ulong userId);
        List<UserSilhouette> GetUsersFollowing(ulong userId);
        List<UserSilhouette> GetBlockedUsers(ulong userId);
        List<UserSilhouette> GetUsersBlocking(ulong userId);

        bool FollowUser(ulong userId, ulong targetUserId);
        bool UnfollowUser(ulong userId, ulong targetUserId);
		bool BlockUser(ulong userId, ulong targetUserId);
		bool UnblockUser(ulong userId, ulong targetUserId);

		bool RateUser(ulong userId, ulong targetUserId, UserRating rating);
        bool RemoveUserRating(ulong userId, ulong targetUserId);
		(int Positive, int Negative) GetUserRatings(ulong userId);
    }

	public interface IProfileOperations
    {
        Task<UserProfile> GetUserProfileAsync(ulong userId, ulong targetId);

        Task<List<EventShard>> GetUserActivityAsync(ulong userId, ulong targetId);
        Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(ulong userId);

        Task<List<UserSilhouette>> GetFriendsAsync(ulong userId);
        Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userId);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId);

        Task FollowUserAsync(ulong userId, ulong targetId);
        Task UnfollowUserAsync(ulong userId, ulong targetId);
        Task BlockUserAsync(ulong userId, ulong targetId);
        Task UnblockUserAsync(ulong userId, ulong targetId);

        Task RateUserAsync(ulong userId, ulong targetId, UserRating rating);
    }
}

