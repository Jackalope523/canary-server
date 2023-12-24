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

    #endregion

    #region Gates

    public interface INotificationDatabase
	{
		Task<List<Note>> GetNotesAsync(ulong userId);
		Task<bool> SaveNoteAsync(ulong userId, ulong notifierId, DateTimeOffset time,
			string message, string action);

		Task<(DeviceType DeviceType, string DeviceToken)> GetUserSubscriptionAsync(ulong userId);
		Task<bool> SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken);
		Task<bool> UnsubscribeUserAsync(ulong userId);
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
