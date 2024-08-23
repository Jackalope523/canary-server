using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;


namespace Repository
{
    public class EFCoreNotificationStore : QueryStore, INotificationDatabase
    {
        public EFCoreNotificationStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task DeleteTelegramAsync(ulong telegramId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Telegrams.Remove(new Telegram { Id = telegramId }));
        }

        public async Task<List<TelegramShard>> GetAllTelegrams(TelegramMessage messageType)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Telegrams.
            Where(n => n.Message == messageType).
            Select(n => new TelegramShard(n.Id, n.NotifierId, n.Time, n.Message, n.Action)).
            ToListAsync());
        }

        public async Task<List<TelegramShard>> GetTelegramsAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Telegrams.
            Where(n => n.RecipientId == userId).
            Select(n => new TelegramShard(n.Id, n.NotifierId, n.Time, n.Message, n.Action)).
            ToListAsync());
        }
        public async Task<DeviceShard> GetUserSubscriptionAsync(ulong userId)
        {
           return await storeSentry.ExecuteReadAsync(ctx =>
           ctx.Subscriptions.
           Where(s => s.UserId == userId).
           Select(s => new DeviceShard(s.DeviceType, s.DeviceToken)).
           SingleAsync());

        }
        public async Task SaveTelegramAsync(ulong recipientId, ulong notifierId, DateTimeOffset time, TelegramMessage message, string context)
        {
            Telegram toAdd = new() 
            {  
                NotifierId = notifierId, 
                RecipientId = recipientId,
                Time = time, 
                Message = message, 
                Action =  context, 
                Read = false
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Telegrams.Add(toAdd));
        }
        public async Task SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
        {
            Subscription toAdd = new()
            {
                UserId = userId,
                DeviceType = deviceType,
                DeviceToken = deviceToken
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Subscriptions.Add(toAdd));
        }
        public async Task UnsubscribeUserAsync(ulong userId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Subscriptions.Where(s => s.UserId == userId).ExecuteDeleteAsync());
        }
    }
}
