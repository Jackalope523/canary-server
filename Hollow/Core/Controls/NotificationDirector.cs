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
		public NotificationDirector(CoreTerminal terminal) : base(terminal) { }

		public async Task SubscribeUserAsync(Guid userID, DeviceType deviceType, string deviceToken)
		{
			Notifications.SubscribeUser(userID, deviceType, deviceToken);
		}

		public async Task UnsubscribeUserAsync(Guid userID)
		{
			Notifications.UnsubscribeUser(userID);
		}


		internal async Task NotifyUserAsync(Guid userID, string title, string message)
		{
			var userSettings = Notifications.GetUserSubscription(userID);
			// Check if user is subscribed
			if  (userSettings == (null, null))
			{ return; }

			await Terminal.NotificationService.PushNotification(userSettings.DeviceType, userSettings.DeviceToken, title, message);
		}
	}
}
