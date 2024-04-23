using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;
using Shared;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class MediaDirector : AbstractDirector, IMediaOperations
	{
		#region Initialisation

		public MediaDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<MemoryStream> GetImageStreamAsync(ulong userId, ulong etchingId)
		{
			var user = await GetUserAsync(userId);
			var etching = await Etchings.GetEtchingAsync(etchingId);
			Entities.User etchingOwner = new(etching.User);
			var etchedEvent = await GetEventAsync(etching.EventId);

			Try(user.Etched(etching) ||
				await user.IsFriendsWith(etchingOwner) ||
				await etchedEvent.WasAttendedBy(user),
				new InvalidUserException("User cannot access this etching."));

			var stream = await Media.DownloadImageAsync(etching.Id, etching.User.Id);

			return stream;
		}

		#endregion

		#region Favours

		public async Task UploadImageAsync(ulong userId, ulong etchingId, MemoryStream image)
		{
			var user = await GetUserAsync(userId);
			var etching = await Etchings.GetEtchingAsync(etchingId);

			await Media.UploadImageAsync(etching.Id, user.Id, image);
		}

		#endregion
	}
}
