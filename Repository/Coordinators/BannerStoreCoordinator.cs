namespace Repository
{
    internal class BannerStoreCoordinator : IBannerDatabase
    {
        private readonly IBannerDatabase store;

        public BannerStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreBannerStore(flag);
        }

        public async Task AddBannerMemberAsync(string phoneNumber, string banner)
        {
            await store.AddBannerMemberAsync(phoneNumber, banner);
        }

        public async Task<string> GetUserBannerAsync(ulong userId)
        {
            return await store.GetUserBannerAsync(userId);
        }

        public async Task<string> GetUserBannerAsync(string phoneNumber)
        {
            return await store.GetUserBannerAsync(phoneNumber);
        }
    }
}
