namespace Repository
{
    public class EFCoreBannerStore : QueryStore, IBannerDatabase
    {
        public EFCoreBannerStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task AddBannerMemberAsync(string phoneNumber, string banner)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserBannerAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserBannerAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}