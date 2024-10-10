using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class MediaDirector : AbstractDirector, IMediaOperations
	{
		#region Initialisation

		public MediaDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<MemoryStream> GetAssetAsync(string asset)
		{
			return await Media.DownloadAssetAsync(asset);
		}

		public async Task<MemoryStream> GetAvatarAsync(ulong userId, ulong targetId)
		{
			var user = await GetUserAsync(userId);
			User targetUser = new() { Id = targetId };

			FailIf(await user.IsBlockedBy(targetUser),
				new InvalidUserException("User cannot access this avatar."));

			var stream = await Media.DownloadAvatarAsync(targetUser.Id);

			return stream;
		}

		public async Task<ImageMetadataShard> GetAvatarMetadataAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            User targetUser = new() { Id = targetId };

            FailIf(await user.IsBlockedBy(targetUser),
                new InvalidUserException("User cannot access this avatar."));

            var image = await Media.DownloadAvatarAsync(targetUser.Id);

            // Get image hash
            var hashSync = ComputeHashAsync(image);

            // Check if image is concealed due to reports
            // TODO Does not yet exist for avatars

            return new(await hashSync, false);
        }

        public async Task<MemoryStream> GetHeaderAsync(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

            Verify(await gathering.IsVisibleTo(user),
                new InvalidUserException("User cannot view this gathering."));

            var stream = await Media.DownloadHeroAsync(gathering.Id);

            return stream;
        }

        public async Task<ImageMetadataShard> GetHeaderMetadataAsync(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

            Verify(await gathering.IsVisibleTo(user),
                new InvalidUserException("User cannot view this gathering."));

            var image = await Media.DownloadHeroAsync(gathering.Id);

            // Get image hash
            var hashSync = ComputeHashAsync(image);

            // Check if image is concealed due to reports
            // TODO

            return new(await hashSync, false);
        }

        public async Task<MemoryStream> GetSnapshotAsync(ulong userId, ulong snapshotId)
        {
            var user = await GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            User snapshotOwner = new(snapshot.User);
            var etchedGathering = await GetGatheringAsync(snapshot.GatheringId);

            Verify(user.Taken(snapshot) ||
                await user.IsCompanionsWith(snapshotOwner) ||
                await etchedGathering.WasAttendedBy(user),
                new InvalidUserException("User cannot access this snapshot."));

            var stream = await Media.DownloadSnapshotAsync(snapshot.Id, snapshot.User.Id);

            return stream;
        }

        public async Task<ImageMetadataShard> GetSnapshotMetadataAsync(ulong userId, ulong snapshotId)
        {
            var user = await GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            User snapshotOwner = new(snapshot.User);
            var etchedGathering = await GetGatheringAsync(snapshot.GatheringId);

            Verify(user.Taken(snapshot) ||
                await user.IsCompanionsWith(snapshotOwner) ||
                await etchedGathering.WasAttendedBy(user),
                new InvalidUserException("User cannot access this snapshot."));

            var image = await Media.DownloadSnapshotAsync(snapshot.Id, snapshot.User.Id);

            // Get image hash
            var hashSync = ComputeHashAsync(image);

            // Check if image is concealed due to reports
            // TODO

            return new(await hashSync, false);
        }

        #endregion

        #region Favours

        public async Task UploadAvatarAsync(ulong userId, MemoryStream image)
		{
			await Media.UploadAvatarAsync(userId, image);
		}

		public async Task UploadHeroAsync(ulong gatheringId, MemoryStream image)
		{
			await Media.UploadHeroAsync(gatheringId, image);
		}

        public async Task UploadSnapshotAsync(ulong userId, ulong snapshotId, MemoryStream image)
		{
			await Media.UploadSnapshotAsync(snapshotId, userId, image);
		}

        #endregion

        #region Tools

        public async Task<string> ComputeHashAsync(MemoryStream image)
        {
            if (image != null)
            {
                image.Seek(0, SeekOrigin.Begin);

                using SHA256 sha256 = SHA256.Create();

                byte[] hashBytes = await sha256.ComputeHashAsync(image);

                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                return hash;
            }
            else
            { throw new UnexpectedFailureException("Image was unable to be retrieved."); }
        }

        #endregion
    }
}
