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

        public async Task<List<TelegramShard>> GetTelegramsAsync(long userId)
        {
            return await store.GetTelegramsAsync(userId);
        }

        public async Task SaveTelegramAsync(long recipientId, long notifierId, DateTimeOffset time, TelegramMessage message, string context)
        {
           await store.SaveTelegramAsync(recipientId, notifierId, time, message, context);
        }

        public async Task DeleteTelegramAsync(long telegramId)
        {
           await store.DeleteTelegramAsync(telegramId);
        }
    }
}
