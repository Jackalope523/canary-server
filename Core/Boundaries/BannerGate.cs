using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schema

    public record CoreBanner(long Id, string Banner, string Colour, string Code)
		: CoreOnlyData();

	public record BannerShard(long Id, string Banner, string Colour);

    #endregion

    #region Gates

    public interface IBannerDatabase
	{
		Task<CoreBanner> FindBannerByIdAsync(long bannerId);
		Task<CoreBanner> FindBannerByCodeAsync(string code);

        Task<CoreBanner> FindBannerForUserAsync(long userId);
		Task AddUserToBannerAsync(long userId, long bannerId, DateTimeOffset time);

		Task<List<UserShard>> GetBannerMembersAsync(long bannerId);
    }

	public interface IBannerOperations
	{
        Task<BannerShard> GetBannerAsync(long userId, long targetId);
		Task<string> GetBannerCodeAsync(long userId);

		Task<List<long>> GetBannerMembersAsync(long userId);
    }

    #endregion
}

