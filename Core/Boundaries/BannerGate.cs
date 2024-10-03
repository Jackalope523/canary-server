using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schema

    public record CoreBanner(ulong Id, string Banner, string Colour, string Code)
		: CoreOnlyData();

	public record BannerShard(ulong Id, string Banner, string Colour);

    #endregion

    #region Gates

    public interface IBannerDatabase
	{
		Task<CoreBanner> FindBannerByIdAsync(ulong bannerId);
		Task<CoreBanner> FindBannerByCodeAsync(string code);

        Task<CoreBanner> FindBannerForUserAsync(ulong userId);
		Task AddUserToBannerAsync(ulong userId, ulong bannerId, DateTimeOffset time);

		Task<List<UserShard>> GetBannerMembersAsync(ulong bannerId);
    }

	public interface IBannerOperations
	{
        Task<BannerShard> GetBannerAsync(ulong userId, ulong targetId);
		Task<string> GetBannerCodeAsync(ulong userId);

		Task<List<ulong>> GetBannerMembersAsync(ulong userId);
    }

    #endregion
}

