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

		public NotificationGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet]
		public async Task<IActionResult> GetNotificationPreferences()
		{
			return await Execute(async user =>
			{
				return await telegrams.GetNotificationPreferencesAsync(user.Id);
			});
        }

		[HttpPost]
		public async Task<IActionResult> UpdateNotificationPreferences()
		{
			return await Execute(async user =>
			{
				await telegrams.UpdateNotificationPreferencesAsync(user.Id);
			});
        }

		#endregion
	}
}