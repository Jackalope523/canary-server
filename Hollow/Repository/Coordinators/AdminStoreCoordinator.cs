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

        public async Task VoidEventAsync(ulong eventId)
        {
            await store.VoidEventAsync(eventId);
        }

        public async Task VoidUserAsync(ulong userId)
        {
           await store.VoidUserAsync(userId);
        }
    }
}
