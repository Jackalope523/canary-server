using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Frontier.Manifests;
using Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.X509;


namespace Frontier.Controllers
{
	[Route("notifications")]
	[ApiController]
	[Authorize]
	public class NotificationAgent : ControllerBase
	{
		enum NotificationError
		{
			MissingInformation,
			CouldNotCompleteRequest
		}

		INotificationOperations notifications;
		UserManager<UserShard> userManager;

		public NotificationAgent(INotificationOperations notificationOperations, UserManager<UserShard> identityUserManager)
		{
			notifications = notificationOperations;
			userManager = identityUserManager;
		}

		[HttpPost]
		public async Task<IActionResult> Subscribe([FromBody] NotificationSubscriptionManifest subscription)
		{
			if (subscription == null || !ModelState.IsValid)
			{
				return BadRequest(NotificationError.MissingInformation.ToString());
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

		private async Task<UserShard> GetCurrentUserAsync()
		{
			return await userManager.GetUserAsync(HttpContext.User);
		}
	}
}
