using Core.Boundaries;

namespace Repository
{
    internal class AccountStoreCoordinator : IAccountDatabase
    {
        private readonly IAccountDatabase store;

        public AccountStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreAccountStore(flag);
        }

        public async Task CreateUserAsync(string phoneNumber, string email, string normalisedEmail, string name, DateTimeOffset dateOfBirth, DateTimeOffset joinDate, Character character)
        {
            await store.CreateUserAsync(phoneNumber, email, normalisedEmail, name, dateOfBirth, joinDate, character);
        }

        public async Task DeleteUserAsync(ulong id)
        {
            await store.DeleteUserAsync(id);
        }

        public async Task<CoreUser> FindUserByIdAsync(ulong id) 
        {
            return await store.FindUserByIdAsync(id);
        }

        public async Task<CoreUser> FindUserByPhoneNumberAsync(string phoneNumber) 
        {
            return await store.FindUserByPhoneNumberAsync(phoneNumber);
        }

        public async Task<CoreUser> FindUserByEmailAsync(string email) 
        {
            return await store.FindUserByEmailAsync(email);
        }

        public async Task<Haunt> GetUserHauntAsync(ulong id)
        {
            return await store.GetUserHauntAsync(id);
        }

        public async Task<RecentLocation> GetRecentUserLocationAsync(ulong id)
        {
            return await store.GetRecentUserLocationAsync(id);
        }    

        public async Task UpdateUserAsync(ulong id, List<(string Property, object Value)> edits)
        {
            await store.UpdateUserAsync(id, edits);
        }

        public async Task UpdateHauntAsync(ulong id, double latitude, double longitude, double radius, int stability)
        {
            await store.UpdateHauntAsync(id, latitude, longitude, radius, stability);
        }

        public async Task UpdateRecentLocationAsync(ulong id, double latitude, double longitude, double radius)
        {
            await store.UpdateRecentLocationAsync(id, latitude, longitude, radius);
        }
    }
}
