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
            await store.UnappreciateUserAsync(selfId, targetId);
        }
        public async Task BlockUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            await store.BlockUserAsync(selfId, targetId, time);
        }
        public async Task UnblockUserAsync(ulong selfId, ulong targetId) 
        {
            await store.UnblockUserAsync(selfId, targetId);
        }
        public async Task<List<UserShard>> GetAppreciatedUsersAsync(ulong id) 
        {
            return await store.GetAppreciatedUsersAsync(id);
        }
        public async Task<List<UserShard>> GetBlockedUsersAsync(ulong id) 
        {
            return await store.GetBlockedUsersAsync(id);
        }
        public async Task<List<UserShard>> GetCompanionsAsync(ulong id)
        {
            return await store.GetCompanionsAsync(id);
        }

        public async Task<List<UserShard>> GetUsersAppreciatingAsync(ulong userId)
        {
            return await store.GetUsersAppreciatingAsync(userId);
        }

        public async Task<List<UserShard>> GetUsersBlockingAsync(ulong userId)
        {
           return await store.GetUsersBlockingAsync(userId);
        }
    }
}
