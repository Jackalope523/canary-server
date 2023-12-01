using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using System.Collections.Generic;

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

		[HttpGet]
		public async Task<IActionResult> GetNotes()
		{
			return await Execute(async user =>
			{
				var notes = await notifications.GetNotesAsync(user.Id);

				return Ok(notes);
			});
        }

		[HttpPost]
		public async Task<IActionResult> Subscribe([FromBody] NotificationSubscriptionManifest subscription)
		{
			// Verify parameters
			if (subscription == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await notifications.SubscribeUserAsync(user.Id, subscription.DeviceType, subscription.DeviceToken);
			}, allowUnverified: true);
		}

		[HttpDelete]
		public async Task<IActionResult> Unsubscribe()
		{
			return await Execute(async user =>
			{
				await notifications.UnsubscribeUserAsync(user.Id);
			}, allowUnverified: true);
		}

		#endregion
	}
}