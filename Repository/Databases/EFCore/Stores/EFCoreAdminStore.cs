using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreAdminStore : QueryStore, IAdminDatabase
    {
        public EFCoreAdminStore(Harbor.Flag flag) : base(flag)
        {

        }

        public async Task VoidGatheringAsync(ulong gatheringId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Gatherings.
                Where(e => e.Id == gatheringId).
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
