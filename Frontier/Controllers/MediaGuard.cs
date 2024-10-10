using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Headers;
using Repository;
using Twilio.TwiML.Voice;
using System.Security.Cryptography;

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

        [HttpGet("assets/{asset}")]
		public async Task<IActionResult> GetAsset(string asset)
        {
            return await ExecuteUnsafe(async () =>
            {
                var user = await GetCurrentUserAsync();

                ThrowIfUnverified(user);

                var imageStream = await media.GetAssetAsync(asset);

                if (imageStream != null)
                {
                    imageStream.Seek(0, SeekOrigin.Begin);

                    return new FileStreamResult(imageStream, "image/jpeg")
                    {
                        FileDownloadName = $"{asset}.png"
                    };
                }

                throw new UnexpectedFailureException("Could not download image.");
            });
        }

		[HttpGet("avatars/{userId}")]
		public async Task<IActionResult> GetAvatar(ulong userId)
        {
            return await ExecuteUnsafe(async () =>
            {
                var user = await GetCurrentUserAsync();

                ThrowIfUnverified(user);

                var imageStream = await media.GetAvatarAsync(user.Id, userId);

                if (imageStream != null)
                {
                    imageStream.Seek(0, SeekOrigin.Begin);

                    return new FileStreamResult(imageStream, "image/jpeg")
                    {
                        FileDownloadName = "avatar.jpg"
                    };
                }

                throw new UnexpectedFailureException("Could not download image.");
            });
        }

		[HttpGet("avatars/{userId}/metadata")]
		public async Task<IActionResult> GetAvatarMetadata(ulong userId)
        {
            return await Execute(user => media.GetAvatarMetadataAsync(user.Id, userId));
        }

		[HttpGet("headers/{gatheringId}")]
		public async Task<IActionResult> GetHeader(ulong gatheringId)
        {
			return await ExecuteUnsafe(async () =>
			{
				var user = await GetCurrentUserAsync();

				ThrowIfUnverified(user);

				var imageStream = await media.GetHeaderAsync(user.Id, gatheringId);

				if (imageStream != null)
				{
					imageStream.Seek(0, SeekOrigin.Begin);

					return new FileStreamResult(imageStream, "image/jpeg")
					{
						FileDownloadName = "header.jpg"
					};
				}
				
				throw new UnexpectedFailureException("Could not download image.");
			});
        }

        [HttpGet("headers/{gatheringId}/metadata")]
        public async Task<IActionResult> GetHeaderMetadata(ulong gatheringId)
        {
            return await Execute(user => media.GetHeaderMetadataAsync(user.Id, gatheringId));
        }

        [HttpGet("snapshots/{snapshotId}")]
		public async Task<IActionResult> GetSnapshotImage(ulong snapshotId)
        {
            return await ExecuteUnsafe(async () =>
            {
                var user = await GetCurrentUserAsync();

                ThrowIfUnverified(user);

                var imageStream = await media.GetSnapshotAsync(user.Id, snapshotId);

                if (imageStream != null)
                {
                    imageStream.Seek(0, SeekOrigin.Begin);

                    return new FileStreamResult(imageStream, "image/jpeg")
                    {
                        FileDownloadName = "snapshot.jpg"
                    };
                }

                throw new UnexpectedFailureException("Could not download image.");
            });
        }

        [HttpGet("snapshots/{snapshotId}/metadata")]
        public async Task<IActionResult> GetSnapshotMetadata(ulong snapshotId)
        {
            return await Execute(user => media.GetSnapshotMetadataAsync(user.Id, snapshotId));
        }

        #endregion
    }
}