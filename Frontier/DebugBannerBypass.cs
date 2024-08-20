using System;
using System.Threading.Tasks;

namespace Frontier
{
	public class DebugBannerBypass : IBannerDatabase
	{
        public Task AddUserToBannerAsync(ulong userId, ulong bannerId)
        {
            return Task.CompletedTask;
        }

        public Task AddUserToBannerAsync(ulong userId, ulong bannerId, DateTimeOffset time)
        {
            return Task.CompletedTask;
        }

        public Task<BannerShard> CheckCode(string code)
        {
            return Task.FromResult<BannerShard>(new(0, "", code));
        }

        public Task<BannerShard> GetUserBannerAsync(ulong userId)
        {
            return Task.FromResult<BannerShard>(new(0, "debug", "code"));
        }
    }
}

