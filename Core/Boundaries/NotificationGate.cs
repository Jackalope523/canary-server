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

	public enum NotificationGroup
	{
        CompanionCommunication,
		CompanionGathering,
		GatheringReminder,
		GatheringAlert,
    }

    public static class NotificationGroupExtensions
    {
        public static string GetString(this NotificationGroup group)
        {
            return group switch
            {
                NotificationGroup.CompanionCommunication => "preference_companion_communications",
                NotificationGroup.CompanionGathering => "preference_companion_gatherings",
                NotificationGroup.GatheringReminder => "preference_gathering_reminders",
                NotificationGroup.GatheringAlert => "preference_gathering_alerts",
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };
        }
    }

    public record TelegramShard(long Id, long NotifierId, DateTimeOffset Time,
		TelegramMessage Message, string Context);

    #endregion

    #region Gates

    public interface INotificationDatabase
    {
        Task<List<TelegramShard>> GetAllTelegramsAsync(TelegramMessage messageType);

        Task<List<TelegramShard>> GetTelegramsAsync(long userId);
		Task SaveTelegramAsync(long recipientId, long notifierId, DateTimeOffset time,
			TelegramMessage message, string context);
		Task DeleteTelegramAsync(long telegramId);
	}

	public interface INotificationOperations
	{
		Task<List<TelegramShard>> GetTelegramsAsync(long userId);
		Task ClearTelegramsAsync(long userId);
		Task ClearTelegramsAsync(long userId, List<long> telegramIds);
	}

	public interface INotificationService
	{
		Task PushNotification(string notificationId, NotificationGroup notificationGroup,
			string title, string message, string collapseId = "");
	}

	#endregion
}
