using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    public record UserProfile(Guid Id, string Name, int Reputation, int NumberOfFollowers);
    public record UserSilhouette(Guid Id, string Name);

    public interface IProfileDatabase
    {
        Task<List<UserSilhouette>> GetFriendsAsync(Guid id);
        Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid id);
        Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid id);

        Task<bool> FollowUserAsync(Guid selfId, Guid targetId);
        Task<bool> UnfollowUserAsync(Guid selfId, Guid targetId);
        Task<bool> BlockUserAsync(Guid selfId, Guid targetId);
        Task<bool> UnblockUserAsync(Guid selfId, Guid targetId);

        Task<bool> RateUserAsync(Guid selfId, Guid targetId, UserRating rating);
        Task<bool> RemoveUserRatingAsync(Guid selfId, Guid targetId);
        Task<(int Positive, int Negative)> GetUserRatingsAsync(Guid id);
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

