using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class AdminStore : QueryStore, IAdminDatabase
    {
        public AdminStore(Sentry sentry) : base(sentry)
        {

        }

        public async Task VoidEventAsync(ulong eventId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Events.
                Where(e => e.Id == eventId).
                ExecuteDeleteAsync());
        }

        public async Task VoidUserAsync(ulong userId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == userId).
                ExecuteDeleteAsync());
        }
    }
}
