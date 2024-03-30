using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Repository
{
    public class AdminStore : QueryStore, IAdminDatabase
    {
        public AdminStore(IDatabaseSentry sentry) : base(sentry)
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
