using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreBannerStore : QueryStore, IBannerDatabase
    {
        public EFCoreBannerStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task AddBannerMemberAsync(string phoneNumber, string banner)
        {
            //ulong userId = await storeSentry.ExecuteReadAsync(ctx => ctx.Users
            //    .Where(u => u.PhoneNumber == phoneNumber)
            //    .Select(u => u.Id)
            //    .SingleAsync());

            //ulong bannerId = await storeSentry.ExecuteReadAsync(ctx => ctx.Banners
            //    .Where(b => b.Name == banner)
            //    .Select(b => b.Id)
            //    .SingleAsync());

            //await storeSentry.ExecuteWriteAsync(ctx => ctx.BannerLinks.Add(new BannerLink { }));

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