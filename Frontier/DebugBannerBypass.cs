using System;
using System.Threading.Tasks;

namespace Frontier
{
	public class DebugBannerBypass : IBannerDatabase
	{
        public Task AddBannerMemberAsync(string phoneNumber, string banner)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetUserBannerAsync(ulong userId)
        {
            return Task.FromResult("debug");
        }

        public Task<string> GetUserBannerAsync(string phoneNumber)
        {
            return Task.FromResult("debug");
        }
    }
}

