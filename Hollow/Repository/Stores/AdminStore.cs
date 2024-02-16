using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository.Stores
{
    internal class AdminStore : QueryStore, IAdminDatabase
    {
        public AdminStore(Sentry sentry) : base(sentry)
        {

        }

        public async Task VoidEventAsync(ulong eventId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Events.
                ExecuteDelete());
        }

        public async Task VoidUserAsync(ulong userId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Users.
               ExecuteDelete());
        }
    }
}
