using Core.Boundaries;

namespace Repository.Coordinators
{
    public class KeyStoreCoordinator : IKeyDatabase
    {
        IKeyDatabase store;

        public KeyStoreCoordinator()
        {
            store = new AzureKeyStore();
        }

        public async Task<string> GetHollowTwilioAuthKeyAsync()
        {
            return await store.GetHollowTwilioAuthKeyAsync();
        }

        public async Task<string> GetHollowTwilioTokenKeyAsync()
        {
            return await store.GetHollowTwilioTokenKeyAsync();
        }

        public async Task<string> GetSparrowMapKeyAsync()
        {
            return await store.GetSparrowMapKeyAsync();
        }
    }
}
