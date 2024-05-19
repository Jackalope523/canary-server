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
	[Route("media")]
	public class MediaGuard : AbstractGuard
	{
		#region Initialisation

		public MediaGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet("{snapshotId}")]
		public async Task<IActionResult> GetImage(ulong snapshotId)
		{
			return await Execute(async user =>
			{
				var image = await media.GetImageStreamAsync(user.Id, snapshotId);

				return image;
			});
        }

		#endregion
	}
}