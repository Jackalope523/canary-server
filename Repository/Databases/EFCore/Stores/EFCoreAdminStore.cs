using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;

namespace Repository
{
    public class EFCoreAdminStore : QueryStore, IAdminDatabase
    {
        public EFCoreAdminStore(Harbor.Flag flag) : base(flag)
        {

        }

        public async Task<List<CoreGathering>> GetAllWaitingGatheringsAsync(DateTimeOffset currentTime)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Gatherings
                .Where(g => g.State == GatheringState.Upcoming && g.StartTime <= currentTime)
                .Join(
                ctx.Users,
                g => g.HostId,
                u => u.Id,
                (g, u) => new CoreGathering
                (
                    g.Id,
                    new UserShard(u.Id, u.Name),
                    g.Name,
                    g.Description,
                    g.StartTime,
                    g.Location.Y,
                    g.Location.X,
                    g.FriendlyLocation,
                    g.EndTime,
                    g.State,
                    g.GroupMinimum,
                    g.GroupMaximum,
                    new CharacterShard(
                    g.Age,
                    g.Extroversion,
                    g.Athleticisme,
                    g.Chaos,
                    g.Competitiveness,
                    g.Industriousness,
                    g.NightOwl,
                    g.Openness),
                    g.Radius,
                    g.IsDynamic,
                    g.IsPendingDeletion,
                    g.NumberOfGuests
                 )).ToListAsync());
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
