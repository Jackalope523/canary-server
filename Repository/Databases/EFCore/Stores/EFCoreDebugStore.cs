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
            storeSentry.ExecuteWrite(ctx => ctx.PostLinks.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.GatheringLinks.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.UserLinks.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.UserReports.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.GatheringReports.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.Posts.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.Notes.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.Subscriptions.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.Penalties.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.Gatherings.ExecuteDelete());
            storeSentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
        }
    }
}
