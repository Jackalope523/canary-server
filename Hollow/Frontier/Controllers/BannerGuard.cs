using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
	[Route("banner")]
	public class BannerGuard : AbstractGuard
	{
		#region Initialisation

		public BannerGuard(ILogger logger,
			UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IBannerOperations bannerOperations,
			IProfileOperations profileOperations, IEventOperations eventOperations,
			IEtchingOperations etchingOperations, IDisciplineOperations disciplineOperations,
			IMediaOperations mediaOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(logger,
				identityUserManager, identitySignInManager,
				accountOperations, bannerOperations,
				profileOperations, eventOperations,
				etchingOperations, disciplineOperations,
				mediaOperations, notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpPost("{phoneNumber}")]
		public async Task<IActionResult> InviteUser(string phoneNumber)
		{
			return await Execute(async user =>
			{
				var banner = await banners.InviteUserAsync(user.Id, phoneNumber);

				await smsService.SendSMSAsync(phoneNumber, $"You have been invited by ${user.Name} to join the Sparrow beta under the ${banner}.");
			});
        }

		#endregion
	}
}