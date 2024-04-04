using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Shared;

namespace Core.Controls
{
    internal class MediaDirector : AbstractDirector, IMediaOperations
	{
		#region Initialisation

		public MediaDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<List<Note>> GetNotesAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);

			return await Notifications.GetNotesAsync(user.Id);
		}

		public async Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
		{
			await Notifications.SubscribeUserAsync(userId, deviceType, deviceToken);
		}

		public async Task UnsubscribeUserAsync(ulong userId)
		{
			await Notifications.UnsubscribeUserAsync(userId);
		}

		#endregion

		#region Favours


		#endregion
	}
}
