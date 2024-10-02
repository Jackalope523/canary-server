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

		INotificationService notificationService;

		public TelegramGuard(GuardBox box, UserManager<CoreUser> aspUserManager,
			INotificationService externalNotificationService) : base(box, aspUserManager)
		{
			notificationService = externalNotificationService;
		}

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
		public async Task<IActionResult> HookPlayerToUser(string playerId)
		{
			if (string.IsNullOrEmpty(playerId))
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await notificationService.TagPlayerAsUser(user.Id, playerId);
			}, allowUnverified: true);
		}

		#endregion
	}
}