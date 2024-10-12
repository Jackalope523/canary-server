using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;

namespace Repository
{
    public class EFCoreNestStore : QueryStore, INestDatabase
    {     
        private InternalUtilityStore internalUtilityStore;

        private static readonly Func<CanaryContext, long, long, UserRelationship.UserLinkType, Task> RemoveLinkOperation =
            EF.CompileAsyncQuery(
                (CanaryContext ctx, long selfId, long otherId, UserRelationship.UserLinkType type) =>
                ctx.UserLinks
                .Where(l => l.SelfId == selfId && l.OtherId == otherId && l.Type == type)
                .ExecuteDelete());

        public EFCoreNestStore(Harbor.Flag flag) : base(flag)
        {
            internalUtilityStore = new(flag);
        }
        
        public async Task AppreciateUserAsync(long selfId, long targetId, DateTimeOffset time) 
        {
            long id = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.SelfId == selfId && l.OtherId == targetId)
                .Select(l => l.Id)
                .SingleOrDefaultAsync()); 


            if (id == 0)
            {
                UserRelationship toAdd = new()
                {
                    SelfId = selfId,
                    OtherId = targetId,
                    Time = time,
                    Type = UserRelationship.UserLinkType.Appreciate
                };

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(toAdd));
            }
            else
            {
                UserRelationship toUpdate = new()
                {
                    Id = id,
                    SelfId = selfId,
                    OtherId = targetId,
                    Time = time,
                    Type = UserRelationship.UserLinkType.Appreciate
                };

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Update(toUpdate));
            }
        }
        public async Task UnappreciateUserAsync(long selfId, long targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserRelationship.UserLinkType.Appreciate));
        }
        public async Task BlockUserAsync(long selfId, long targetId, DateTimeOffset time) 
        {
            long id = await storeSentry.ExecuteReadAsync(ctx =>
               ctx.UserLinks.
               Where(l => l.SelfId == selfId && l.OtherId == targetId)
               .Select(l => l.Id)
               .SingleOrDefaultAsync());


            if (id == 0)
            {
                UserRelationship toAdd = new()
                {
                    SelfId = selfId,
                    OtherId = targetId,
                    Time = time,
                    Type = UserRelationship.UserLinkType.Block
                };

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(toAdd));
            }
            else
            {
                UserRelationship toUpdate = new()
                {
                    Id = id,
                    SelfId = selfId,
                    OtherId = targetId,
                    Time = time,
                    Type = UserRelationship.UserLinkType.Block
                };

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Update(toUpdate));
            }
        }
        public async Task UnblockUserAsync(long selfId, long targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserRelationship.UserLinkType.Block));
        }
        public async Task<List<UserShard>> GetAppreciatedUsersAsync(long id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Appreciate).
             Join(
                 ctx.Users,
                 l => l.OtherId,
                 u => u.Id,
                 (l, u) => new UserShard(u.Id, u.Name)
                 ).
             ToListAsync());
        }
        public async Task<List<UserShard>> GetBlockedUsersAsync(long id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Block).
            Join(
                ctx.Users, 
                l => l.OtherId, 
                u => u.Id, 
                (l,u) => new UserShard(u.Id, u.Name)
                ).
            ToListAsync());
        }
        public async Task<List<UserShard>> GetCompanionsAsync(long id)
        {
            Task<List<UserShard>> appreciating = storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Appreciate).
             Join(
                 ctx.Users,
                 l => l.OtherId,
                 u => u.Id,
                 (l, u) => new UserShard(u.Id, u.Name)
                 ).
             ToListAsync());

            Task<List<UserShard>> appreciatingMe = storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserLinks.Where(l => l.OtherId == id && l.Type == UserRelationship.UserLinkType.Appreciate).
             Join(
                 ctx.Users,
                 l => l.SelfId,
                 u => u.Id,
                 (l, u) => new UserShard(u.Id, u.Name)
                 ).
             ToListAsync());

            return (await appreciating).Intersect(await appreciatingMe).ToList();
        }

        public async Task<List<UserShard>> GetUsersAppreciatingAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserLinks.Where(l => l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Appreciate).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new UserShard(u.Id, u.Name)).
            ToListAsync());
        }

        public async Task<List<UserShard>> GetUsersBlockingAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserLinks.Where(l => l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Block).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new UserShard(u.Id, u.Name)).
            ToListAsync());
        }

        public Task<bool> HaveMutualGathering(long userId, long targetId)
        {
            return storeSentry.ExecuteReadAsync(ctx => 
                ctx.GatheringLinks.
                Where(l => l.UserId == userId).
                Join(
                      ctx.GatheringLinks.
                      Where(l => l.UserId == targetId),
                      x => x.GatheringId,
                      y => y.GatheringId,
                      (x,y) => x.GatheringId
                    ).
                AnyAsync());
        }

        public async Task<CoreGathering> GetFirstMutualGathering(long userId, long targetId)
        {
            List<long> a = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
                Where(l => l.UserId == userId).
                Select(l => l.GatheringId).
                ToListAsync());

            List<long> b = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
                Where(l => l.UserId == targetId).
                Select(l => l.GatheringId).
                ToListAsync());

            List<long> mutualGatherings = a.Intersect(b).ToList();

            CoreGathering firstMutualGathering = await storeSentry.ExecuteReadAsync(ctx => ctx.Gatherings.
                Where(g => mutualGatherings.Contains(g.Id)).
                OrderByDescending(g => g.StartTime).
                Select(g => new CoreGathering
                (
                   g.Id,
                   new UserShard(g.HostId ?? 0, null),
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
                   g.NumberOfGuests,
                   g.DegreeOfPrivacy
                   )).
                FirstAsync());

            UserShard host = await internalUtilityStore.GetHostShard(firstMutualGathering.Host.Id);

            return firstMutualGathering with { Host = host };
        }

        public async Task<CoreGathering> GetLatestMutualGathering(long userId, long targetId)
        {
            List<long> a = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
               Where(l => l.UserId == userId).
               Select(l => l.GatheringId).
               ToListAsync());

            List<long> b = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
                Where(l => l.UserId == targetId).
                Select(l => l.GatheringId).
                ToListAsync());

            List<long> mutualGatherings = a.Intersect(b).ToList();

            CoreGathering latestMutualGathering = await storeSentry.ExecuteReadAsync(ctx => ctx.Gatherings.
                Where(g => mutualGatherings.Contains(g.Id)).
                OrderBy(g => g.StartTime).
                Select(g => new CoreGathering
                (
                   g.Id,
                   new UserShard(g.HostId ?? 0, null),
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
                   g.NumberOfGuests,
                   g.DegreeOfPrivacy
                   )).
                FirstAsync());

            UserShard host = await internalUtilityStore.GetHostShard(latestMutualGathering.Host.Id);

            return latestMutualGathering with { Host = host };
        }

        public async Task<DateTimeOffset> BlockedSince(long userId, long targetId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
                    ctx.UserLinks.
                    Where(l => l.SelfId == userId && l.OtherId == targetId && l.Type == UserRelationship.UserLinkType.Block).
                    Select(l => l.Time).
                    SingleAsync());
        }
    }
}
