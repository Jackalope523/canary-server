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

		public async Task<BannerShard> GetBannerAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);
			var userBanner = await Banners.FindBannerForUserAsync(user.Id);

			return userBanner;
		}

		#endregion

		#region Favours

		public async Task<BannerShard> RequestUserBannerAsync(User user)
		{
			return await Banners.FindBannerForUserAsync(user.Id);
		}

		#endregion
	}
}
