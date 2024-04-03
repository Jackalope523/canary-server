using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreAdminStore : QueryStore, IAdminDatabase
    {
        public EFCoreAdminStore(Harbor.Flag flag) : base(flag)
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
