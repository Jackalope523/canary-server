using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class BannerDirector : AbstractDirector, IBannerOperations
	{
		#region Initialisation

		public BannerDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<string> InviteUserAsync(ulong userId, string invitedPhoneNumber)
		{
			var user = await GetUserAsync(userId);
			var userBanner = await Banners.GetUserBannerAsync(user.Id);

			var validNumber = ContentValidation.TryNormalisePhoneNumber(invitedPhoneNumber, out invitedPhoneNumber);

			Try(validNumber,
				new InvalidUserException("Invalid phone number."));

			// Check if invited user already has a banner
			try
			{
				await Banners.GetUserBannerAsync(invitedPhoneNumber);
			}
			catch { throw new InvalidUserException("User already has a banner."); }

			// Add user to the banner
			await Banners.AddBannerMemberAsync(invitedPhoneNumber, userBanner);

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
