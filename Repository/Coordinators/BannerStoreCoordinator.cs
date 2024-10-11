namespace Repository
{
    internal class BannerStoreCoordinator : IBannerDatabase
    {
        private readonly IBannerDatabase store;

        public BannerStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreBannerStore(flag);
        }

        public async Task AddUserToBannerAsync(long userId, long bannerId, DateTimeOffset time)
        {
            await store.AddUserToBannerAsync(userId, bannerId, time);
        }

        public async Task<CoreBanner> FindBannerByCodeAsync(string code)
        {
            return await store.FindBannerByCodeAsync(code);
        }

        public async Task<CoreBanner> FindBannerByIdAsync(long bannerId)
        {
            return await store.FindBannerByIdAsync(bannerId);
        }

        public async Task<CoreBanner> FindBannerForUserAsync(long userId)
        {
            return await store.FindBannerForUserAsync(userId);
        }

        public async Task<List<UserShard>> GetBannerMembersAsync(long bannerId)
        {
            return await store.GetBannerMembersAsync(bannerId);
        }
    }
}
