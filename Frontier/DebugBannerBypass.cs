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

        public Task<CoreBanner> FindBannerByCodeAsync(string code)
        {
            return Task.FromResult<CoreBanner>(new(0, "", code));
        }

        public Task<CoreBanner> FindBannerForUserAsync(ulong userId)
        {
            return Task.FromResult<CoreBanner>(new(0, "debug", "code"));
        }
    }
}

