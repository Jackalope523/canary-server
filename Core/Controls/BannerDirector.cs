using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

namespace Core.Controls
{
    internal class BannerDirector : AbstractDirector, IBannerOperations
	{
		#region Initialisation

		public BannerDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<CoreBanner> GetBannerAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);
			var userBanner = await Banners.FindBannerForUserAsync(user.Id);

			return userBanner;
		}

		public async Task<List<ulong>> GetBannerMembersAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);
			var userBanner = await Banners.FindBannerForUserAsync(user.Id);

			return (await Banners.GetBannerMembersAsync(userBanner.Id))
				.ConvertAll(member => member.Id);
		}

		#endregion

		#region Favours

		public async Task<CoreBanner> RequestUserBannerAsync(User user)
		{
			return await Banners.FindBannerForUserAsync(user.Id);
		}

		#endregion
	}
}
