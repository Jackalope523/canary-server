namespace Repository
{
    internal class BannerStoreCoordinator : IBannerDatabase
    {
        private readonly IBannerDatabase store;

        public BannerStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreBannerStore(flag);
        }

        public async Task AddUserToBannerAsync(ulong userId, ulong bannerId)
        {
            await store.AddUserToBannerAsync(userId, bannerId);
        }

        public async Task<BannerShard> CheckCode(string code)
        {
            return await store.CheckCode(code);
        }

        public async Task<BannerShard> GetUserBannerAsync(ulong userId)
        {
            return await store.GetUserBannerAsync(userId);
        }
    }
}
