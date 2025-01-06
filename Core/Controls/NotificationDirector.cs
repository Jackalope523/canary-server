using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Core.Notifications;
using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class NotificationDirector : AbstractDirector, INotificationOperations
	{
		#region Initialisation

		public NotificationDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<List<TelegramShard>> GetTelegramsAsync(long userId)
		{
			var user = await GetUserAsync(userId);

			return await Telegrams.GetTelegramsAsync(user.Id);
		}

		public async Task ClearTelegramsAsync(long userId)
		{
			var user = await GetUserAsync(userId);

			var telegrams = await Telegrams.GetTelegramsAsync(user.Id);
			foreach (var telegram in telegrams)
			{
				try
				{
					await Telegrams.DeleteTelegramAsync(telegram.Id);
				}
				catch { }
			}
		}

		public async Task ClearTelegramsAsync(long userId, List<long> telegramIds)
		{
			var user = await GetUserAsync(userId);

			foreach (var id in telegramIds)
			{
				try
				{
					await Telegrams.DeleteTelegramAsync(id);
				}
				catch { }
			}
		}

		#endregion

		#region Favours

		internal async Task<NotificationProfile> RequestNotificationProfileAsync(User user)
		{
			return await Telegrams.GetNotificationProfileAsync(user.Id);
		}

		internal async Task PostTelegramAsync(User user, User notifier, TelegramMessage message, string context)
		{
			// Check if notifier can notify user
			if (await notifier.IsBlocking(user) || await notifier.IsBlockedBy(user))
			{ return; }

			await Telegrams.SaveTelegramAsync(user.Id, notifier.Id, Time, message, context);
		}

		internal async Task NotifyUserAsync(User user, CanaryNotification notification)
		{
            await Terminal.NotificationService.PushNotification(await user.NotificationProfile, notification);
		}

		#endregion
	}
}
