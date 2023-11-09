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
		(DeviceType DeviceType, string DeviceToken) GetUserSubscription(ulong id);
		bool SubscribeUser(ulong id, DeviceType deviceType, string deviceToken);
		bool UnsubscribeUser(ulong id);
	}

	public interface INotificationOperations
	{
		Task SubscribeUserAsync(ulong userID, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(ulong userID);
	}

	public interface INotificationService
	{
		Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message);
	}
}
