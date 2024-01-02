using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Shared;

namespace Repository
{
    public class NotificationStore : QueryStore, INotificationDatabase
    {
        public static INotificationDatabase Access => new NotificationStore(new TestSentry());

        public NotificationStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<List<Core.Boundaries.Note>> GetNotesAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Notes.
            Where(n => n.NotifierId == userId).
            Select(n => new Core.Boundaries.Note(n.NotifierId, n.Time, n.Message, n.Action)).
            ToListAsync());
        }
        public async Task<DeviceSilhouette> GetUserSubscriptionAsync(ulong userId)
        {
           return await storeSentry.ExecuteReadAsync(ctx =>
           ctx.Subscriptions.
           Where(s => s.UserId == userId).
           Select(s => new DeviceSilhouette(s.DeviceType, s.DeviceToken)).
           SingleAsync());

        }
        public async Task<bool> SaveNoteAsync(ulong notifierId, ulong recipientId, DateTimeOffset time, string message, string action)
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

            return true;
        }
        public async Task<bool> SubscribeUserAsync(ulong userId, DeviceType deviceType, string deviceToken)
        {
            Subscription toAdd = new()
            {
                UserId = userId,
                DeviceType = deviceType,
                DeviceToken = deviceToken
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Subscriptions.Add(toAdd));

            return true;
        }
        public async Task<bool> UnsubscribeUserAsync(ulong userId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Subscriptions.Where(s => s.UserId == userId).ExecuteDeleteAsync());
            return true;
        }
    }
}
