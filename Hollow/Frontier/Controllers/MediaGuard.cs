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

		public MediaGuard(GuardBox box, UserManager<UserShard> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet("{etchingId}")]
		public async Task<IActionResult> GetImage(ulong etchingId)
		{
			return await Execute(async user =>
			{
				var image = await media.GetImageStreamAsync(user.Id, etchingId);

				return image;
			});
        }

		#endregion
	}
}