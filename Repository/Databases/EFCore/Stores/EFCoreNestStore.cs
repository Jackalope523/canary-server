using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreNestStore : QueryStore, INestDatabase
    {     
        private static readonly Func<CanaryContext, ulong, ulong, UserRelationship.UserLinkType, Task> RemoveLinkOperation =
            EF.CompileAsyncQuery(
                (CanaryContext ctx, ulong selfId, ulong otherId, UserRelationship.UserLinkType type) =>
                ctx.UserLinks
                .Where(l => l.SelfId == selfId && l.OtherId == otherId && l.Type == type)
                .ExecuteDelete());

        public EFCoreNestStore(Harbor.Flag flag) : base(flag)
        {
        }
        
        public async Task AppreciateUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            ulong id = await storeSentry.ExecuteReadAsync(ctx => 
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
        public async Task UnappreciateUserAsync(ulong selfId, ulong targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserRelationship.UserLinkType.Appreciate));
        }
        public async Task BlockUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            ulong id = await storeSentry.ExecuteReadAsync(ctx =>
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
        public async Task UnblockUserAsync(ulong selfId, ulong targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserRelationship.UserLinkType.Block));
        }
        public async Task<List<UserShard>> GetAppreciatedUsersAsync(ulong id) 
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
        public async Task<List<UserShard>> GetBlockedUsersAsync(ulong id) 
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
        public async Task<List<UserShard>> GetCompanionsAsync(ulong id)
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

        public async Task<List<UserShard>> GetUsersAppreciatingAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserLinks.Where(l => l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Appreciate).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new UserShard(u.Id, u.Name)).
            ToListAsync());
        }

        public async Task<List<UserShard>> GetUsersBlockingAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserLinks.Where(l => l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Block).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new UserShard(u.Id, u.Name)).
            ToListAsync());
        }

        public Task<bool> HaveMutualGathering(ulong userId, ulong targetId)
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
    }
}
