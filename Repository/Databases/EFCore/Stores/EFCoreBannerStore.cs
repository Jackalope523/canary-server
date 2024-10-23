using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreBannerStore : QueryStore, IBannerDatabase
    {
        public EFCoreBannerStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task AddUserToBannerAsync(long userId, long bannerId, DateTimeOffset time)
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
               .Select(b => new CoreBanner(b.Id, b.Name, b.Color, b.Code))
               .SingleAsync());
        }

        public async Task<CoreBanner> FindBannerByIdAsync(long bannerId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
              ctx.Banners
              .Where(b => b.Id == bannerId)
              .Select(b => new CoreBanner(b.Id, b.Name, b.Color, b.Code))
              .SingleAsync());
        }

        public async Task<CoreBanner> FindBannerForUserAsync(long userId)
        {
            long bannerId = await storeSentry.ExecuteReadAsync(ctx =>
                               ctx.BannerLinks
                               .Where(l => l.UserId == userId)
                               .Select(l => l.BannerId)
                               .SingleAsync());

            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Banners
                .Where(b => b.Id == bannerId)
                .Select(b => new CoreBanner(b.Id, b.Name, b.Color, b.Code))
                .SingleAsync());
        }

        public async Task<List<UserShard>> GetBannerMembersAsync(long bannerId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.BannerLinks
                .Where(l => l.BannerId == bannerId)
                .Join(
                    ctx.Users.Where(u => u.SoftDeleted != true), 
                    l => l.UserId, 
                    u => u.Id, 
                    (l,u) => new UserShard(u.Id, u.Name))
                .ToListAsync());
        }
    }
}