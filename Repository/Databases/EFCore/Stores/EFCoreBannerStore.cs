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

        public async Task<BannerShard> FindBannerByCodeAsync(string code)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
               ctx.Banners
               .Where(b => b.Code == code)
               .Select(b => new BannerShard(b.Id, b.Name, b.Code, b.Color))
               .SingleAsync());
        }

        public async Task<BannerShard> FindBannerByIdAsync(ulong bannerId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
              ctx.Banners
              .Where(b => b.Id == bannerId)
              .Select(b => new BannerShard(b.Id, b.Name, b.Code, b.Color))
              .SingleAsync());
        }

        public async Task<BannerShard> FindBannerForUserAsync(ulong userId)
        {
            ulong bannerId = await storeSentry.ExecuteReadAsync(ctx =>
                               ctx.BannerLinks
                               .Where(l => l.UserId == userId)
                               .Select(l => l.BannerId)
                               .SingleAsync());

            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Banners
                .Where(b => b.Id == bannerId)
                .Select(b => new BannerShard(b.Id, b.Name, b.Code, b.Color))
                .SingleAsync());
        }

        public async Task<List<UserShard>> GetBannerMembersAsync(ulong bannerId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.BannerLinks
                .Where(l => l.BannerId == bannerId)
                .Join(
                    ctx.Users.Where(u => u.IsPendingDeletion != true), 
                    l => l.UserId, 
                    u => u.Id, 
                    (l,u) => new UserShard(u.Id, u.Name))
                .ToListAsync());
        }
    }
}