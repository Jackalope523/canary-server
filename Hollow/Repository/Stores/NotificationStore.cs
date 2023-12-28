
using Core.Boundaries;
using Shared;

namespace Repository
{
    public class NotificationStore : QueryStore, INotificationDatabase
    {
        public static INotificationDatabase Access => new NotificationStore(new TestSentry());

        public NotificationStore(Sentry sentry) : base(sentry)
        {
        }

        public Task<List<Note>> GetNotesAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<(DeviceType DeviceType, string DeviceToken)> GetUserSubscriptionAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveNoteAsync(ulong userId, ulong notifierId, DateTimeOffset time, string message, string action)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnsubscribeUserAsync(ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
