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
		// APP SEGMENT (0XXX)
		UserAgreementsUpdated = 0001,
		ServerMaintenance = 0002,

		// ACCOUNT SEGMENT (1XXX)
		AccountStatusChanged = 1001,

		// USER SEGMENT (2XXX)
		UserAppreciated = 2001,

		// GATHERING SEGMENT (3XXX)
		GatheringInvitation = 3001,

		GatheringClosingSoon = 3100,

		GatheringMissedHost = 3200,
		GatheringMissedAttendee = 3201,
		GatheringSealed = 3202,
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
