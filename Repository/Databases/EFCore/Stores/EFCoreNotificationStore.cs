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

        public async Task<List<NoteShard>> GetNotesAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Notes.
            Where(n => n.RecipientId == userId).
            Select(n => new NoteShard(n.NotifierId, n.Time, n.Message, n.Action)).
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
        public async Task SaveNoteAsync(ulong recipientId, ulong notifierId, DateTimeOffset time, string message, string action)
        {
            Entities.Note toAdd = new() 
            {  
                NotifierId = notifierId, 
                RecipientId = recipientId,
                Time = time, 
                Message = message, 
                Action =  action, 
                Read = false
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Notes.Add(toAdd));
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
