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

		[HttpGet("avatars/{userId}")]
		public async Task<IActionResult> GetAvatar(ulong userId)
		{
			return await Execute(async user =>
			{
				var image = await media.GetAvatarAsync(user.Id, userId);

				return image;
			});
        }

		[HttpGet("heros/{gatheringId}")]
		public async Task<IActionResult> GetHero(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				var image = await media.GetHeroAsync(user.Id, gatheringId);

				return image;
			});
        }

		[HttpGet("snapshots/{snapshotId}")]
		public async Task<IActionResult> GetSnapshotImage(ulong snapshotId)
		{
			return await Execute(async user =>
			{
				var image = await media.GetSnapshotAsync(user.Id, snapshotId);

				return image;
			});
        }

		#endregion
	}
}