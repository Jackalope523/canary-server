using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

using static Core.Entities.Psijic;

namespace Core.Controls
{
	internal class NotificationDirector : AbstractDirector, INotificationOperations
	{
		#region Initialisation

		public NotificationDirector(CoreTerminal terminal) : base(terminal) { }

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

		internal async Task PostNoteAsync(User user, User notifier, string message, string action)
		{
			// Check if notifier can notify user
			if (await notifier.IsBlocking(user) || await notifier.IsBlockedBy(user))
			{ return; }

			await Notifications.SaveNoteAsync(user.Id, notifier.Id, Time, message, action);
		}

		internal async Task NotifyUserAsync(User user, string title, string message)
		{
			DeviceSilhouette userSettings;

            // Check if user is subscribed
            try
            {
				userSettings = await Notifications.GetUserSubscriptionAsync(user.Id);
            }
			catch (Exception)
			{
				return;
			}
				
            await Terminal.NotificationService.PushNotification(userSettings.DeviceType, userSettings.DeviceToken, title, message);
		}

		#endregion
	}
}
