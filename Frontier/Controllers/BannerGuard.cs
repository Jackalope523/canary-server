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

		public BannerGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet]
		public async Task<IActionResult> GetBanner()
		{
			return await Execute(async user => await banners.GetBannerAsync(user.Id), allowUnverified: true);
        }

		[HttpGet("members")]
		public async Task<IActionResult> GetMembers()
		{
			return await Execute(async user => await banners.GetBannerMembersAsync(user.Id), allowUnverified: true);
		}

		#endregion
	}
}