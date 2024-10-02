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

        public async Task<List<TelegramShard>> GetAllTelegramsAsync(TelegramMessage messageType)
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
    }
}
