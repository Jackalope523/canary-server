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

        public async Task<BannerShard> CheckCodeAsync(string code)
        {
            return await store.CheckCodeAsync(code);
        }

        public async Task<BannerShard> GetUserBannerAsync(ulong userId)
        {
            return await store.GetUserBannerAsync(userId);
        }
    }
}
