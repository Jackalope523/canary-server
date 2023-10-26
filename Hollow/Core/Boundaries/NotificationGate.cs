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
		(DeviceType DeviceType, string DeviceToken) GetUserSubscription(Guid id);
		bool SubscribeUser(Guid id, DeviceType deviceType, string deviceToken);
		bool UnsubscribeUser(Guid id);
	}

	public interface INotificationOperations
	{
		Task SubscribeUserAsync(Guid userID, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(Guid userID);
	}

	public interface INotificationService
	{
		Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message);
	}
}
