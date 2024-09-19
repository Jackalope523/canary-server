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
	[Route("telegrams")]
	public class TelegramGuard : AbstractGuard
	{
		#region Initialisation

		public TelegramGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet]
		public async Task<IActionResult> GetTelegrams()
		{
			return await Execute(async user =>
			{
				return await telegrams.GetTelegramsAsync(user.Id);
			});
        }

		[HttpPut]
		public async Task<IActionResult> ClearTelegram([FromBody] List<ulong> telegramIds)
		{
			return await Execute(async user =>
			{
				await telegrams.ClearTelegramsAsync(user.Id, telegramIds);
			});
        }

		[HttpDelete]
		public async Task<IActionResult> ClearTelegrams()
		{
			return await Execute(async user =>
			{
				await telegrams.ClearTelegramsAsync(user.Id);
			});
        }

		[HttpPost("push")]
		public async Task<IActionResult> Subscribe([FromBody] NotificationSubscriptionManifest subscription)
		{
			// Verify parameters
			if (subscription == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await telegrams.SubscribeUserAsync(user.Id, subscription.DeviceType, subscription.DeviceToken);
			}, allowUnverified: true);
		}

		[HttpDelete("push")]
		public async Task<IActionResult> Unsubscribe()
		{
			return await Execute(async user =>
			{
				await telegrams.UnsubscribeUserAsync(user.Id);
			}, allowUnverified: true);
		}

		#endregion
	}
}