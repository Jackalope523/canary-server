using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    public class EFCoreDebugStore : QueryStore, IDebugDatabase
    {
        public EFCoreDebugStore(Harbor.Flag flag) : base(flag)
        {

        }

        public async Task DrainDatabaseAsync()
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.PostLinks.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventLinks.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventReports.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Notes.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Subscriptions.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Penalties.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Events.ExecuteDelete());
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.ExecuteDelete());
        }
    }
}
