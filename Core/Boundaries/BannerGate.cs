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
		Task<BannerShard> CheckCodeAsync(string code);

        Task<BannerShard> GetUserBannerAsync(ulong userId);
		Task AddUserToBannerAsync(ulong userId, ulong bannerId, DateTimeOffset time);
    }

	public interface IBannerOperations
	{
        Task<BannerShard> GetBannerAsync(ulong userId);
    }

    #endregion
}

