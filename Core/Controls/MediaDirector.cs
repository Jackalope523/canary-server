using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class MediaDirector : AbstractDirector, IMediaOperations
	{
		#region Initialisation

		public MediaDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<MemoryStream> GetImageStreamAsync(ulong userId, ulong snapshotId)
		{
			var user = await GetUserAsync(userId);
			var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
			Entities.User snapshotOwner = new(snapshot.User);
			var etchedGathering = await GetGatheringAsync(snapshot.GatheringId);

			Try(user.Etched(snapshot) ||
				await user.IsCompanionsWith(snapshotOwner) ||
				await etchedGathering.WasAttendedBy(user),
				new InvalidUserException("User cannot access this snapshot."));

			var stream = await Media.DownloadImageAsync(snapshot.Id, snapshot.User.Id);

			return stream;
		}

		#endregion

		#region Favours

		public async Task UploadImageAsync(ulong userId, ulong snapshotId, MemoryStream image)
		{
			var user = await GetUserAsync(userId);
			var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);

			await Media.UploadImageAsync(snapshot.Id, user.Id, image);
		}

		#endregion
	}
}
