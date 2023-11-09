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
        List<UserSilhouette> GetFriends(ulong id);
        List<UserSilhouette> GetFollowedUsers(ulong id);
        List<UserSilhouette> GetUsersFollowing(ulong id);
        List<UserSilhouette> GetBlockedUsers(ulong id);
        List<UserSilhouette> GetUsersBlocking(ulong id);

        bool FollowUser(ulong selfId, ulong targetId);
        bool UnfollowUser(ulong selfId, ulong targetId);
        bool BlockUser(ulong selfId, ulong targetId);
        bool UnblockUser(ulong selfId, ulong targetId);

        bool RateUser(ulong selfId, ulong targetId, UserRating rating);
        bool RemoveUserRating(ulong selfId, ulong targetId);
        (int Positive, int Negative) GetUserRatings(ulong id);
    }

	public interface IProfileOperations
    {
        Task<UserProfile> GetUserProfileAsync(ulong userID, ulong targetID);

        Task<List<EventShard>> GetUserActivityAsync(ulong userID, ulong targetID);
        Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(ulong userID);

        Task<List<UserSilhouette>> GetFriendsAsync(ulong userID);
        Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userID);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userID);

        Task FollowUserAsync(ulong userID, ulong targetID);
        Task UnfollowUserAsync(ulong userID, ulong targetID);
        Task BlockUserAsync(ulong userID, ulong targetID);
        Task UnblockUserAsync(ulong userID, ulong targetID);

        Task RateUserAsync(ulong userID, ulong targetID, UserRating rating);
    }
}

