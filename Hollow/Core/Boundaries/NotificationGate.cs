using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

	public record Note(ulong NotifierId, DateTimeOffset Time,
		string Message, string Action);

	public record DeviceSilhouette(DeviceType DeviceType, string DeviceToken);

    #endregion

    #region Gates

    public interface INotificationDatabase
	{
		Task<List<Note>> GetNotesAsync(ulong userId);
		Task SaveNoteAsync(ulong recipientId, ulong notifierId, DateTimeOffset time,
			string message, string action);

		Task<DeviceSilhouette> GetUserSubscriptionAsync(ulong userId);
		Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(ulong userId);
	}

	public interface INotificationOperations
	{
		Task<List<Note>> GetNotesAsync(ulong userId);

		Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken);
		Task UnsubscribeUserAsync(ulong userId);
	}

	public interface INotificationService
	{
		Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message);
	}

	#endregion
}
