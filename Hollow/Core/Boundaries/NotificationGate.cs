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
		List<Note> GetNotes(ulong userId);
		bool SaveNote(ulong userId, ulong notifierId, DateTimeOffset time,
			string message, string action);

		(DeviceType DeviceType, string DeviceToken) GetUserSubscription(ulong userId);
		bool SubscribeUser(ulong userId, DeviceType deviceType, string deviceToken);
		bool UnsubscribeUser(ulong userId);
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
