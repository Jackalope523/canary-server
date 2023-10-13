using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Boundaries
{
    public record UserProfile(Guid Id, string Name, int Reputation, int NumberOfFollowers);
    public record UserSilhouette(Guid Id, string Name);

    public interface IProfileDatabase
    {
        List<UserSilhouette> GetFriends(Guid id);
        List<UserSilhouette> GetFollowedUsers(Guid id);
        List<UserSilhouette> GetBlockedUsers(Guid id);

        bool FollowUser(Guid selfId, Guid targetId);
        bool UnfollowUser(Guid selfId, Guid targetId);
        bool BlockUser(Guid selfId, Guid targetId);
        bool UnblockUser(Guid selfId, Guid targetId);

        bool RateUser(Guid selfId, Guid targetId, UserRating rating);
        bool RemoveUserRating(Guid selfId, Guid targetId);
        (int Positive, int Negative) GetUserRatings(Guid id);
    }

	public interface IProfileOperations
    {
        Task<UserProfile> GetUserProfileAsync(Guid userID, Guid targetID);

        Task<List<EventShard>> GetUserActivityAsync(Guid userID, Guid targetID);
        Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(Guid userID);

        Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid userID);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid userID);

        Task FollowUserAsync(Guid userID, Guid targetID);
        Task UnfollowUserAsync(Guid userID, Guid targetID);
        Task BlockUserAsync(Guid userID, Guid targetID);
        Task UnblockUserAsync(Guid userID, Guid targetID);

        Task RateUserAsync(Guid userID, Guid targetID, UserRating rating);
    }
}

