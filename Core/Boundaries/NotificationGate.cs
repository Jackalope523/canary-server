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
		UserAgreementsUpdated,
		AccountStatusChanged,

		UserAppreciated,

		GatheringInvitation,

		GatheringClosingSoon,
		GatheringMissedHost,
		GatheringMissedAttendee,
	}

    public enum DeviceType
    { iOS, Android }

    public record TelegramShard(ulong Id, ulong NotifierId, DateTimeOffset Time,
		TelegramMessage Message, string Context);

    #endregion

    #region Gates

    public interface INotificationDatabase
    {
        Task<List<TelegramShard>> GetAllTelegramsAsync(TelegramMessage messageType);

        Task<List<TelegramShard>> GetTelegramsAsync(ulong userId);
		Task SaveTelegramAsync(ulong recipientId, ulong notifierId, DateTimeOffset time,
			TelegramMessage message, string context);
		Task DeleteTelegramAsync(ulong telegramId);
	}

	public interface INotificationOperations
	{
		Task<List<TelegramShard>> GetTelegramsAsync(ulong userId);
		Task ClearTelegramsAsync(ulong userId);
		Task ClearTelegramsAsync(ulong userId, List<ulong> telegramIds);
	}

	public interface INotificationService
	{
		Task PushNotification(string notificationId, string title, string message);
	}

	#endregion
}
