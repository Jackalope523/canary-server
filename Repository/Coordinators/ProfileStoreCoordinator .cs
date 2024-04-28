using Core.Boundaries;


namespace Repository
{
    public class ProfileStoreCoordinator : IProfileDatabase
    {
        private readonly IProfileDatabase store;

        public ProfileStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreProfileStore(flag);
        }
        
        public async Task FollowUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            await store.FollowUserAsync(selfId, targetId, time);
        }
        public async Task UnfollowUserAsync(ulong selfId, ulong targetId) 
        {
            await store.UnblockUserAsync(selfId, targetId);
        }
        public async Task BlockUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            await store.BlockUserAsync(selfId, targetId, time);
        }
        public async Task UnblockUserAsync(ulong selfId, ulong targetId) 
        {
            await store.UnblockUserAsync(selfId, targetId);
        }
        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong id) 
        {
            return await store.GetFollowedUsersAsync(id);
        }
        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong id) 
        {
            return await store.GetBlockedUsersAsync(id);
        }
        public async Task<List<UserSilhouette>> GetFriendsAsync(ulong id)
        {
            return await store.GetFriendsAsync(id);
        }

        public async Task<(int Positive, int Negative)> GetUserRatingsAsync(ulong id)
        {
            return await store.GetUserRatingsAsync(id);
        }

        public async Task RateUserAsync(ulong selfId, ulong targetId, UserRating rating, DateTimeOffset time)
        {
            await store.RateUserAsync(selfId, targetId, rating, time);
        }

        public async Task RemoveUserRatingAsync(ulong selfId, ulong targetId)
        {
            await store.RemoveUserRatingAsync(selfId, targetId);
        }

        public async Task<List<UserSilhouette>> GetUsersFollowingAsync(ulong userId)
        {
            return await store.GetUsersFollowingAsync(userId);
        }

        public async Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userId)
        {
           return await store.GetUsersBlockingAsync(userId);
        }
    }
}
