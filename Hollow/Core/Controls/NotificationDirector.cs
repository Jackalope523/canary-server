using Core.Boundaries;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controls
{
	internal class NotificationDirector : AbstractDirector, INotificationOperations
	{
		#region Initialisation

		public NotificationDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
		{
			Notifications.SubscribeUser(userId, deviceType, deviceToken);
		}

		public async Task UnsubscribeUserAsync(ulong userId)
		{
			Notifications.UnsubscribeUser(userId);
		}

		#endregion

		#region Favours

		internal async Task NotifyUserAsync(ulong userId, string title, string message)
		{
			var userSettings = Notifications.GetUserSubscription(userId);
			// Check if user is subscribed
			if  (userSettings == (null, null))
			{ return; }

			await Terminal.NotificationService.PushNotification(userSettings.DeviceType, userSettings.DeviceToken, title, message);
		}

		#endregion
	}
}
