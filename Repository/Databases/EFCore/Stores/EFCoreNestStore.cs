using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreNestStore : QueryStore, INestDatabase
    {     
        private static readonly Func<CanaryContext, long, long, UserRelationship.UserLinkType, Task> RemoveLinkOperation =
            EF.CompileAsyncQuery(
                (CanaryContext ctx, long selfId, long otherId, UserRelationship.UserLinkType type) =>
                ctx.UserLinks
                .Where(l => l.SelfId == selfId && l.OtherId == otherId && l.Type == type)
                .ExecuteDelete());

        public EFCoreNestStore(Harbor.Flag flag) : base(flag)
        {
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
    }
}
