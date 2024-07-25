using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

	public enum TelegramMessage
	{
		UserAppreciated, GatheringInvitation
	}

    public enum DeviceType
    { iOS, Android }

    public record TelegramShard(ulong NotifierId, DateTimeOffset Time,
		TelegramMessage Message, string Context);

	public record DeviceShard(DeviceType DeviceType, string DeviceToken);

    #endregion

    #region Gates

    public interface INotificationDatabase
	{
		Task<List<TelegramShard>> GetTelegramsAsync(ulong userId);
		Task SaveTelegramAsync(ulong recipientId, ulong notifierId, DateTimeOffset time,
			TelegramMessage message, string context);

		Task<DeviceShard> GetUserSubscriptionAsync(ulong userId);
		Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(ulong userId);
	}

	public interface INotificationOperations
	{
		Task<List<TelegramShard>> GetTelegramsAsync(ulong userId);

		Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(ulong userId);
	}

	public interface INotificationService
	{
		Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message);
	}

	#endregion
}
