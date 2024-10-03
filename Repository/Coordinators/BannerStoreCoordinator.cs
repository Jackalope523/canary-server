namespace Repository
{
    internal class BannerStoreCoordinator : IBannerDatabase
    {
        private readonly IBannerDatabase store;

        public BannerStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreBannerStore(flag);
        }

        public async Task AddUserToBannerAsync(ulong userId, ulong bannerId, DateTimeOffset time)
        {
            await store.AddUserToBannerAsync(userId, bannerId, time);
        }

        public async Task<BannerShard> FindBannerByCodeAsync(string code)
        {
            return await store.FindBannerByCodeAsync(code);
        }

        public async Task<BannerShard> FindBannerByIdAsync(ulong bannerId)
        {
            return await store.FindBannerByIdAsync(bannerId);
        }

        public async Task<BannerShard> FindBannerForUserAsync(ulong userId)
        {
            return await store.FindBannerForUserAsync(userId);
        }

        public async Task<List<UserShard>> GetBannerMembersAsync(ulong bannerId)
        {
            return await store.GetBannerMembersAsync(bannerId);
        }
    }
}
