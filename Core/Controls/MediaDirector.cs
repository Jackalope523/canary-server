using System.IO;
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

		public async Task<MemoryStream> GetAvatarAsync(ulong userId, ulong otherId)
		{
			var user = await GetUserAsync(userId);
			User otherUser = new() { Id = otherId };

			Fail(await user.IsBlockedBy(otherUser),
				new InvalidUserException("User cannot access this avatar."));

			var stream = await Media.DownloadAvatarAsync(otherUser.Id);

			return stream;
		}

		public async Task<MemoryStream> GetHeroAsync(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

            Try(await gathering.IsVisibleTo(user),
                new InvalidUserException("User cannot view this gathering."));

            var stream = await Media.DownloadHeroAsync(gathering.Id);

            return stream;
        }

        public async Task<MemoryStream> GetSnapshotAsync(ulong userId, ulong snapshotId)
        {
            var user = await GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            User snapshotOwner = new(snapshot.User);
            var etchedGathering = await GetGatheringAsync(snapshot.GatheringId);

            Try(user.Taken(snapshot) ||
                await user.IsCompanionsWith(snapshotOwner) ||
                await etchedGathering.WasAttendedBy(user),
                new InvalidUserException("User cannot access this snapshot."));

            var stream = await Media.DownloadSnapshotAsync(snapshot.Id, snapshot.User.Id);

            return stream;
        }

        #endregion

        #region Favours

		public async Task UploadAvatarAsync(ulong userId, MemoryStream image)
		{
			await Media.UploadAvatarAsync(userId, image);
		}

		public async Task UploadHeroAsync(ulong gatheringId, MemoryStream image)
		{
			await Media.UploadAvatarAsync(gatheringId, image);
		}

        public async Task UploadSnapshotAsync(ulong userId, ulong snapshotId, MemoryStream image)
		{
			await Media.UploadSnapshotAsync(snapshotId, userId, image);
		}

		#endregion
	}
}
