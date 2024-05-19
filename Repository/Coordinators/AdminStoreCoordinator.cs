using Core.Boundaries;

namespace Repository
{
    public class AdminStoreCoordinator : IAdminDatabase
    {
        private readonly IAdminDatabase store;

        public AdminStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreAdminStore(flag);
        }

        public async Task VoidGatheringAsync(ulong gatheringId)
        {
            await store.VoidGatheringAsync(gatheringId);
        }

        public async Task VoidUserAsync(ulong userId)
        {
           await store.VoidUserAsync(userId);
        }
    }
}
