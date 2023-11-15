using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	public interface INotificationDatabase
	{
		(DeviceType DeviceType, string DeviceToken) GetUserSubscription(ulong userId);
		bool SubscribeUser(ulong userId, DeviceType deviceType, string deviceToken);
		bool UnsubscribeUser(ulong userId);
	}

	public interface INotificationOperations
	{
		Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(ulong userId);
	}

	public interface INotificationService
	{
		Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message);
	}
}
