using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreBannerStore : QueryStore, IBannerDatabase
    {
        public EFCoreBannerStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task AddUserToBannerAsync(ulong userId, ulong bannerId, DateTimeOffset time)
        {
            BannerLink toAdd = new()
            {
                UserId = userId,
                BannerId = bannerId,
                Time = time,
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.BannerLinks.Add(toAdd));
        }

        public async Task<CoreBanner> FindBannerByCodeAsync(string code)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
               ctx.Banners
               .Where(b => b.Code == code)
               .Select(b => new CoreBanner(b.Id, b.Name, b.Code))
               .SingleAsync());
        }

        public async Task<CoreBanner> FindBannerForUserAsync(ulong userId)
        {
            ulong bannerId = await storeSentry.ExecuteReadAsync(ctx =>
                               ctx.BannerLinks
                               .Where(l => l.UserId == userId)
                               .Select(l => l.BannerId)
                               .SingleAsync());

            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Banners
                .Where(b => b.Id == bannerId)
                .Select(b => new CoreBanner(b.Id, b.Name, b.Code))
                .SingleAsync());
        }
    }
}