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
                        FileDownloadName = $"{asset}.jpg"
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
                        FileDownloadName = "hero.jpg"
                    };
                }

                throw new UnexpectedFailureException("Could not download image.");
            });
        }

		[HttpGet("avatars/{userId}/hash")]
		public async Task<IActionResult> GetAvatarHash(ulong userId)
        {
            return await Execute(async user =>
            {
                var imageStream = await media.GetAvatarAsync(user.Id, userId);

                if (imageStream != null)
                {
                    imageStream.Seek(0, SeekOrigin.Begin);

                    using SHA256 sha256 = SHA256.Create();

                    byte[] hashBytes = sha256.ComputeHash(imageStream);

                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }

                throw new UnexpectedFailureException("Could not get image hash.");
            });
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

        [HttpGet("headers/{gatheringId}/hash")]
        public async Task<IActionResult> GetHeaderHash(ulong gatheringId)
        {
            return await Execute(async user =>
            {
                var imageStream = await media.GetHeaderAsync(user.Id, gatheringId);

                if (imageStream != null)
                {
                    imageStream.Seek(0, SeekOrigin.Begin);

                    using SHA256 sha256 = SHA256.Create();

                    byte[] hashBytes = sha256.ComputeHash(imageStream);

                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }

                throw new UnexpectedFailureException("Could not get image hash.");
            });
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
                        FileDownloadName = "hero.jpg"
                    };
                }

                throw new UnexpectedFailureException("Could not download image.");
            });
        }

		#endregion
	}
}