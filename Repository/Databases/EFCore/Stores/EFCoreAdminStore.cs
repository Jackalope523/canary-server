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
                    g.Title,
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
                    g.SoftDeleted,
                    g.NumberOfGuests,
                    g.DegreeOfPrivacy
                 )).ToListAsync());
        }

        public async Task VoidGatheringAsync(long gatheringId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
             ctx.GatheringLinks.
             Where(l => l.GatheringId == gatheringId).
             ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
             ctx.GatheringReports.
             Where(r => r.GatheringId == gatheringId).
             ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
              ctx.UserReports.
              Where(r => r.GatheringId == gatheringId).
              ExecuteDeleteAsync());

            List<long> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                                     ctx.Snapshots.
                                     Where(s => s.GatheringId == gatheringId).
                                     Select(s => s.Id).
                                     ToListAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
             ctx.SnapshotLinks.
             Where(l => snapshots.Contains(l.SnapshotId)).
             ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
             ctx.Snapshots.
             Where(s => s.GatheringId == gatheringId).
             ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Gatherings.
                Remove(new Gathering { Id = gatheringId }));
        }

        public async Task VoidUserAsync(long userId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.BannerLinks.
                Where(l => l.UserId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Penalties.
                Where(p => p.PenalizedId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.SnapshotLinks.
                Where(l => l.UserId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Snapshots.
                Where(s => s.OwnerId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Subscriptions.
                Where(s => s.UserId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Telegrams.
                Where(t => t.RecipientId == userId || t.NotifierId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.UserReports.
                Where(r => r.SelfId == userId || r.OtherId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.GatheringReports.
                Where(r => r.UserId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.UserRelationships.
                Where(l => l.SelfId == userId || l.OtherId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.GatheringLinks.
                Where(l => l.UserId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Gatherings.
                Where(e => e.HostId == userId).
                ExecuteDeleteAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Users.
                Remove(new User { Id = userId }));
        }
    }
}
