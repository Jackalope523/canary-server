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

		public async Task<BannerShard> GetBannerAsync(ulong userId, ulong targetId)
		{
			var user = await GetUserAsync(userId);
			var targetUser = await GetUserAsync(targetId);

			return (await targetUser.Banner).ToBannerShard();
		}

		public async Task<string> GetBannerCodeAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);

			return (await user.Banner).Code;
		}

		public async Task<List<ulong>> GetBannerMembersAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);

			return (await (await user.Banner).Members)
				.ConvertAll(member => member.Id);
		}

		#endregion

		#region Favours

		public async Task<Banner> RequestUserBannerAsync(User user)
		{
			return new(await Banners.FindBannerForUserAsync(user.Id));
		}

		public async Task<List<User>> RequestBannerMembersAsync(Banner banner)
		{
			return (await Banners.GetBannerMembersAsync(banner.Id))
				.ConvertAll(user => new User(user));
		}

		#endregion
	}
}
