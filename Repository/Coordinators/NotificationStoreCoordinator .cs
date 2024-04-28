using Core.Boundaries;


namespace Repository
{
    public class NotificationStoreCoordinator : INotificationDatabase
    {
        private readonly INotificationDatabase store;
        public NotificationStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreNotificationStore(flag);
        }

        public async Task<List<NoteShard>> GetNotesAsync(ulong userId)
        {
            return await store.GetNotesAsync(userId);
        }

        public async Task<DeviceSilhouette> GetUserSubscriptionAsync(ulong userId)
        {
            return await store.GetUserSubscriptionAsync(userId);
        }

        public async Task SaveNoteAsync(ulong recipientId, ulong notifierId, DateTimeOffset time, string message, string action)
        {
           await store.SaveNoteAsync(recipientId, notifierId, time, message, action);
        }

        public async Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
        {
           await store.SubscribeUserAsync(userId, deviceType, deviceToken);
        }

        public async Task UnsubscribeUserAsync(ulong userId)
        {
           await store.UnsubscribeUserAsync(userId);
        }
    }
}
