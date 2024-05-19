using Core.Boundaries;


namespace Repository
{
    public class NestStoreCoordinator : INestDatabase
    {
        private readonly INestDatabase store;

        public NestStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreNestStore(flag);
        }
        
        public async Task AppreciateUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            await store.AppreciateUserAsync(selfId, targetId, time);
        }
        public async Task UnappreciateUserAsync(ulong selfId, ulong targetId) 
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
        public async Task<List<UserSilhouette>> GetAppreciatedUsersAsync(ulong id) 
        {
            return await store.GetAppreciatedUsersAsync(id);
        }
        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong id) 
        {
            return await store.GetBlockedUsersAsync(id);
        }
        public async Task<List<UserSilhouette>> GetCompanionsAsync(ulong id)
        {
            return await store.GetCompanionsAsync(id);
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

        public async Task<List<UserSilhouette>> GetUsersAppreciatingAsync(ulong userId)
        {
            return await store.GetUsersAppreciatingAsync(userId);
        }

        public async Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userId)
        {
           return await store.GetUsersBlockingAsync(userId);
        }
    }
}
