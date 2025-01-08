using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Core.Notifications;

using static Core.Entities.Psijic;
using static Core.Entities.Artificer;

namespace Core.Controls
{
    internal class NotificationDirector : AbstractDirector, INotificationOperations
	{
		#region Initialisation

		public NotificationDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<NotificationPreferencesShard> GetNotificationPreferencesAsync(long userId)
		{
			var user = await GetUserAsync(userId);
			var profile = await user.NotificationProfile;

			return new(profile.NotificationId, profile.SocialInvitation, profile.CompanionActivity,
				profile.GatheringActivity, profile.GatheringReminder, profile.GatheringDiscovery);
		}

        public async Task UpdateNotificationPreferencesAsync(long userId,
            bool? socialInvitation = null, bool? companionActivity = null,
			bool? gatheringReminder = null, bool? gatheringActivity = null,
			bool? gatheringDiscovery = null)
		{
			var user = await GetUserAsync(userId);

            List<(string Property, object Value)> edits = new();

            if (IsNotNull(socialInvitation))
			{
                edits.Add((nameof(NotificationProfile.SocialInvitation), socialInvitation.Value));
            }
            if (IsNotNull(companionActivity))
			{
                edits.Add((nameof(NotificationProfile.CompanionActivity), companionActivity.Value));
            }
            if (IsNotNull(gatheringReminder))
			{
                edits.Add((nameof(NotificationProfile.GatheringReminder), gatheringReminder.Value));
            }
            if (IsNotNull(gatheringActivity))
			{
                edits.Add((nameof(NotificationProfile.GatheringActivity), gatheringActivity.Value));
            }
            if (IsNotNull(gatheringDiscovery))
			{
                edits.Add((nameof(NotificationProfile.GatheringDiscovery), gatheringDiscovery.Value));
            }

			await Telegrams.UpdateNotificationProfileAsync(user.Id, edits);
		}

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

			var telegrams = await Telegrams.GetTelegramsAsync(user.Id);

			foreach (var id in telegramIds)
			{
				// Check if user owns telegram
				// TODO Potential flag for abuse
				if (telegrams.Exists(t => t.Id.Equals(id)))
				{
					try
					{
						await Telegrams.DeleteTelegramAsync(id);
					}
					catch { }
				}

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

		internal async Task<string> NotifyUserAsync(User user, CanaryNotification notification, DateTimeOffset? notifyAt = null)
		{
			string notificationId;

			if (IsNotNull(notifyAt))
			{
                notificationId = await Terminal.NotificationService.ScheduleNotification(notification, notifyAt.Value, await user.NotificationProfile);
			}
			else
			{
				notificationId = await Terminal.NotificationService.DispatchNotification(notification, await user.NotificationProfile);
            }

			return notificationId;
        }

		internal async Task<string> NotifyUsersAsync(CanaryNotification notification, DateTimeOffset? notifyAt = null, params User[] users)
		{
			var profiles = await Task.WhenAll(users.Select(async user => await user.NotificationProfile));

			string notificationId;

			if (IsNotNull(notifyAt))
			{
				notificationId = await Terminal.NotificationService.ScheduleNotification(notification, notifyAt.Value, profiles);
			}
			else
			{
				notificationId = await Terminal.NotificationService.DispatchNotification(notification, profiles);
			}

			return notificationId;
		}

		internal async Task CancelScheduledNotifications(params string[] notificationIds)
		{
			foreach (var id in notificationIds)
			{
				await Terminal.NotificationService.CancelNotification(id);
			}
		}

		#endregion
	}
}
