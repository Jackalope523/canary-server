namespace Repository
{
    public class NestStoreCoordinator : INestDatabase
    {
        private readonly INestDatabase store;

        public NestStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreNestStore(flag);
        }
        
        public async Task AppreciateUserAsync(long selfId, long targetId, DateTimeOffset time) 
        {
            await store.AppreciateUserAsync(selfId, targetId, time);
        }
        public async Task UnappreciateUserAsync(long selfId, long targetId) 
        {
            await store.UnappreciateUserAsync(selfId, targetId);
        }
        public async Task BlockUserAsync(long selfId, long targetId, DateTimeOffset time) 
        {
            await store.BlockUserAsync(selfId, targetId, time);
        }
        public async Task UnblockUserAsync(long selfId, long targetId) 
        {
            await store.UnblockUserAsync(selfId, targetId);
        }
        public async Task<List<UserShard>> GetAppreciatedUsersAsync(long id) 
        {
            return await store.GetAppreciatedUsersAsync(id);
        }
        public async Task<List<UserShard>> GetBlockedUsersAsync(long id) 
        {
            return await store.GetBlockedUsersAsync(id);
        }
        public async Task<List<UserShard>> GetCompanionsAsync(long id)
        {
            return await store.GetCompanionsAsync(id);
        }

        public async Task<List<UserShard>> GetUsersAppreciatingAsync(long userId)
        {
            return await store.GetUsersAppreciatingAsync(userId);
        }

        public async Task<List<UserShard>> GetUsersBlockingAsync(long userId)
        {
           return await store.GetUsersBlockingAsync(userId);
        }
        public async Task<bool> HaveMutualGathering(long userId, long targetId)
        {
            return await store.HaveMutualGathering(userId, targetId);
        }
    }
}
