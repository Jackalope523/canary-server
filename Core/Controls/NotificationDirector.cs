using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class NotificationDirector : AbstractDirector, INotificationOperations
	{
		#region Initialisation

		public NotificationDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<List<TelegramShard>> GetTelegramsAsync(ulong userId)
		{
			var user = await GetUserAsync(userId);

			return await Telegrams.GetTelegramsAsync(user.Id);
		}

		public async Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
		{
			await Telegrams.SubscribeUserAsync(userId, deviceType, deviceToken);
		}

		public async Task UnsubscribeUserAsync(ulong userId)
		{
			await Telegrams.UnsubscribeUserAsync(userId);
		}

		#endregion

		#region Favours

		internal async Task PostTelegramAsync(User user, User notifier, TelegramMessage message, string context)
		{
			// Check if notifier can notify user
			if (await notifier.IsBlocking(user) || await notifier.IsBlockedBy(user))
			{ return; }

			await Telegrams.SaveTelegramAsync(user.Id, notifier.Id, Time, message, context);
		}

		internal async Task NotifyUserAsync(User user, string title, string message)
		{
			DeviceShard userSettings;

            // Check if user is subscribed
            try
            {
				userSettings = await Telegrams.GetUserSubscriptionAsync(user.Id);
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
