using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Notifications;

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

    public record TelegramShard(long Id, long NotifierId, DateTimeOffset Time,
		TelegramMessage Message, string Context);

	public record NotificationProfile(long UserId, Guid NotificationId,
		bool SocialInvitation, bool CompanionActivity,
		bool GatheringReminder, bool GatheringActivity,
		bool GatheringDiscovery);

	public record NotificationPreferencesShard(Guid NotificationId,
		bool SocialInvitation, bool CompanionActivity,
		bool GatheringReminder, bool GatheringActivity,
		bool GatheringDiscovery);

	public record HostNotificationSchedule(long GatheringId, string GatheringWaitingId);

	public record GuestNotificationSchedule(long UserId, long GatheringId,
		string GatheringUpcomingId, string GatheringImminentId);

    #endregion

    #region Gates

    public interface INotificationDatabase
    {
		Task<NotificationProfile> GetNotificationProfileAsync(long userId);
        Task UpdateNotificationProfileAsync(long userId, List<(string Property, object Value)> edits);

		Task<(HostNotificationSchedule, List<GuestNotificationSchedule>)> GetGatheringNotificationScheduleAsync(long gatheringId);
		Task UpdateGatheringHostNotificationScheduleAsync(string gatheringWaitingId);
		Task UpdateGatheringGuestNotificationSchedulesAsync(params (long userId, string gatheringUpcomingId, string gatheringImminentId)[] guestSchedules);
		Task ClearGatheringNotificationScheduleAsync();

        Task<List<TelegramShard>> GetAllTelegramsAsync(TelegramMessage messageType);

        Task<List<TelegramShard>> GetTelegramsAsync(long userId);
		Task SaveTelegramAsync(long recipientId, long notifierId, DateTimeOffset time,
			TelegramMessage message, string context);
		Task DeleteTelegramAsync(long telegramId);
	}

	public interface INotificationOperations
	{
		Task<NotificationPreferencesShard> GetNotificationPreferencesAsync(long userId);
		Task UpdateNotificationPreferencesAsync(long userId);

		Task<List<TelegramShard>> GetTelegramsAsync(long userId);
		Task ClearTelegramsAsync(long userId);
		Task ClearTelegramsAsync(long userId, List<long> telegramIds);
	}

	public interface INotificationService
	{
		Task PushNotification(NotificationProfile userNotificationProfile, CanaryNotification notification);
	}

	#endregion
}
