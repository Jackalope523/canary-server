using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schema

    public record BannerShard(ulong Id, string Banner, string Code);

	#endregion

	#region Gates

	public interface IBannerDatabase
	{
		Task<BannerShard> FindBannerByCodeAsync(string code);

        Task<BannerShard> FindBannerForUserAsync(ulong userId);
		Task AddUserToBannerAsync(ulong userId, ulong bannerId, DateTimeOffset time);
    }

	public interface IBannerOperations
	{
        Task<BannerShard> GetBannerAsync(ulong userId);
    }

    #endregion
}

