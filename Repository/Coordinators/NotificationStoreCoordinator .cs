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

        public async Task<List<TelegramShard>> GetAllTelegramsAsync(TelegramMessage messageType)
        {
            return await store.GetAllTelegramsAsync(messageType);
        }

        public async Task<List<TelegramShard>> GetTelegramsAsync(ulong userId)
        {
            return await store.GetTelegramsAsync(userId);
        }

        public async Task<DeviceShard> GetUserSubscriptionAsync(ulong userId)
        {
            return await store.GetUserSubscriptionAsync(userId);
        }

        public async Task SaveTelegramAsync(ulong recipientId, ulong notifierId, DateTimeOffset time, TelegramMessage message, string context)
        {
           await store.SaveTelegramAsync(recipientId, notifierId, time, message, context);
        }

        public async Task DeleteTelegramAsync(ulong telegramId)
        {
           await store.DeleteTelegramAsync(telegramId);
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
