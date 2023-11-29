using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;

namespace Frontier.Controllers
{
	[Route("notifications")]
	public class NotificationAgent : AbstractAgent
	{
		#region Initialisation

		public NotificationAgent(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IReportOperations reportOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				reportOperations, notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpPost]
		public async Task<IActionResult> Subscribe([FromBody] NotificationSubscriptionManifest subscription)
		{
			if (subscription == null || !ModelState.IsValid)
			{
				return BadRequest(HollowError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				await notifications.SubscribeUserAsync(user.Id, subscription.DeviceType, subscription.DeviceToken);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpDelete]
		public async Task<IActionResult> Unsubscribe()
		{
			try
			{
				var user = await GetCurrentUserAsync();
				await notifications.UnsubscribeUserAsync(user.Id);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		#endregion
	}
}