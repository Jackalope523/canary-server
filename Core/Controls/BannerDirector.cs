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
			var userBanner = await Banners.GetUserBannerAsync(user.Id);

			return userBanner;
		}

		#endregion

		#region Favours

		public async Task<string> RequestUserBannerAsync(User user)
		{
			return await Banners.GetUserBannerAsync(user.Id);
		}

		#endregion
	}
}
