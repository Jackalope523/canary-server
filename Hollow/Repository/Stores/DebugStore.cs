using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class DebugStore : QueryStore, IDebugDatabase
    {
        public DebugStore(Sentry sentry) : base(sentry)
        {

        }

        public async Task DrainDatabaseAsync()
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventLinks.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventReports.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.PostLinks.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Subscriptions.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Notes.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Penalties.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Events.ExecuteDeleteAsync());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.ExecuteDeleteAsync());
        }
    }
}