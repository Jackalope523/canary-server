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
			return await Execute(async user => new { });
        }

		[HttpPut]
		public async Task<IActionResult> ClearTelegram([FromBody] List<long> telegramIds)
		{
			return await Execute(async user => "");
        }

		[HttpDelete]
		public async Task<IActionResult> ClearTelegrams()
		{
			return await Execute(async user => "");
        }

		#endregion
	}
}