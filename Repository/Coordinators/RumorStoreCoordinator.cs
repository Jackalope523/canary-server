
namespace Repository
{
    internal class RumorStoreCoordinator : IRumorDatabase
    {
        private readonly IRumorDatabase store;

        public RumorStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreRumorStore(flag);
        }

        public async Task<CoreRumor> CreateRumorAsync(long rumoredGatheringId, long authorId, string text, DateTimeOffset time)
        {
            return await store.CreateRumorAsync(rumoredGatheringId, authorId, text, time);
        }

        public async Task<CoreRumoredGathering> CreateRumoredGatheringAsync(double latitude, double longitude, string friendlyLocation)
        {
            return await store.CreateRumoredGatheringAsync(latitude, longitude, friendlyLocation);
        }

        public async Task<UserShard> GetFounderAsync(long rumoredGatheringId)
        {
            return await store.GetFounderAsync(rumoredGatheringId);
        }

        public async Task<CoreRumor> GetRumorAsync(long id)
        {
            return await store.GetRumorAsync(id);
        }

        public async Task<CoreRumoredGathering> GetRumoredGatheringAsync(long id)
        {
            return await store.GetRumoredGatheringAsync(id);
        }

        public async Task<List<CoreRumor>> GetRumorsAboutAsync(long rumoredGatheringId)
        {
            return await store.GetRumorsAboutAsync(rumoredGatheringId);
        }

        public async Task<List<CoreRumor>> GetRumorsByAsync(long userId)
        {
            return await store.GetRumorsByAsync(userId);
        }

        public async Task<List<CoreRumor>> GetRumorsByCompanionsOfAsync(long userId)
        {
            return await store.GetRumorsByCompanionsOfAsync(userId);
        }

        public async Task<List<(CoreRumor, string)>> GetWallRumorsAsync(long userId)
        {
            return await store.GetWallRumorsAsync(userId);
        }

        public async Task HardDeleteRumorAsync(long id)
        {
            await store.HardDeleteRumorAsync(id);
        }

        public async Task HardDeleteRumoredGatheringAsync(long id)
        {
            await store.HardDeleteRumoredGatheringAsync(id);
        }

        public async Task SoftDeleteRumorAsync(long id)
        {
            await store.SoftDeleteRumorAsync(id);
        }

        public async Task SoftDeleteRumoredGatheringAsync(long id)
        {
            await store.SoftDeleteRumoredGatheringAsync(id);
        }

        public async Task UpdateRumorAsync(long id, List<(string Property, object Value)> edits)
        {
            await store.UpdateRumorAsync(id, edits);
        }

        public async Task UpdateRumoredGatheringAsync(long id, List<(string Property, object Value)> edits)
        {
            await store.UpdateRumoredGatheringAsync(id, edits);
        }
    }
}
