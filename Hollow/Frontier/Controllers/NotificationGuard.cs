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
	[Route("notifications")]
	public class NotificationGuard : AbstractGuard
	{
		#region Initialisation

		public NotificationGuard(ILogger logger,
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

		[HttpGet]
		public async Task<IActionResult> GetNotes()
		{
			return await Execute(async user =>
			{
				var manifest = ManifestSeries<NoteManifest>.Create(
					await notifications.GetNotesAsync(user.Id),
					note => new NoteManifest(note));

				return manifest;
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