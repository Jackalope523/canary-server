using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository
{
    public class EFCoreProfileStore : QueryStore, IProfileDatabase
    {     
        private static readonly Func<QueryContext, ulong, ulong, UserLink.UserLinkType, Task> RemoveLinkOperation =
            EF.CompileAsyncQuery(
                (QueryContext ctx, ulong selfId, ulong otherId, UserLink.UserLinkType type) =>
                ctx.UserLinks
                .Where(l => l.SelfId == selfId && l.OtherId == otherId && l.Type == type)
                .ExecuteDelete());

        public EFCoreProfileStore(Harbor.Flag flag) : base(flag)
        {
        }
        
        public async Task FollowUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            UserLink toAdd = new()
            {
                SelfId = selfId,
                OtherId = targetId,
                Time = time,
                Type = UserLink.UserLinkType.Follow
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(toAdd));
        }
        public async Task UnfollowUserAsync(ulong selfId, ulong targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserLink.UserLinkType.Follow));
        }
        public async Task BlockUserAsync(ulong selfId, ulong targetId, DateTimeOffset time) 
        {
            UserLink toAdd = new()
            {
                SelfId = selfId,
                OtherId = targetId,
                Time = time,
                Type = UserLink.UserLinkType.Block
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(toAdd));      
        }
        public async Task UnblockUserAsync(ulong selfId, ulong targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserLink.UserLinkType.Block));
        }
        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow).
             Join(
                 ctx.Users,
                 l => l.OtherId,
                 u => u.Id,
                 (l, u) => new UserSilhouette(u.Id, u.Name)
                 ).
             ToListAsync());
        }
        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Block).
            Join(
                ctx.Users, 
                l => l.OtherId, 
                u => u.Id, 
                (l,u) => new UserSilhouette(u.Id, u.Name)
                ).
            ToListAsync());
        }
        public async Task<List<UserSilhouette>> GetFriendsAsync(ulong id)
        {
            Task<List<UserSilhouette>> following = storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow).
             Join(
                 ctx.Users,
                 l => l.OtherId,
                 u => u.Id,
                 (l, u) => new UserSilhouette(u.Id, u.Name)
                 ).
             ToListAsync());

            Task<List<UserSilhouette>> followingMe = storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserLinks.Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow).
             Join(
                 ctx.Users,
                 l => l.SelfId,
                 u => u.Id,
                 (l, u) => new UserSilhouette(u.Id, u.Name)
                 ).
             ToListAsync());

            return (await following).Intersect(await followingMe).ToList();
        }

        public async Task<(int Positive, int Negative)> GetUserRatingsAsync(ulong id)
        {
            Task<int> up = storeSentry.ExecuteReadAsync(ctx =>
            ctx.UserLinks.
            Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.RateUp).
            CountAsync());

            Task<int> down = storeSentry.ExecuteReadAsync(ctx =>
            ctx.UserLinks.
            Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.RateDown).
            CountAsync()); ;

            return (await up, await down);
        }

        public async Task RateUserAsync(ulong selfId, ulong targetId, UserRating rating, DateTimeOffset time)
        {
            UserLink.UserLinkType type;
            if (rating.Equals(UserRating.Positive)) type = UserLink.UserLinkType.RateUp;
            else type = UserLink.UserLinkType.RateDown;

            UserLink toAdd = new()
            {
                SelfId = selfId,
                OtherId = targetId,
                Time = time,
                Type = type
            };

            ulong id = await storeSentry.ExecuteReadAsync(ctx =>
                        ctx.UserLinks.
                        Where(l => l.SelfId == selfId && l.OtherId == targetId).
                        Select(l => l.Id).
                        SingleOrDefaultAsync());

            if (id != 0)
            {
                toAdd.Id = id;
            }

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Update(toAdd));
        }

        public async Task RemoveUserRatingAsync(ulong selfId, ulong targetId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => 
            ctx.UserLinks.
            Where(l => 
            l.SelfId == selfId && l.OtherId == targetId && 
            (l.Type == UserLink.UserLinkType.RateUp || l.Type == UserLink.UserLinkType.RateDown)).
            ExecuteDelete());
        }

        public async Task<List<UserSilhouette>> GetUsersFollowingAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserLinks.Where(l => l.OtherId == userId && l.Type == UserLink.UserLinkType.Follow).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new UserSilhouette(u.Id, u.Name)).
            ToListAsync());
        }

        public async Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserLinks.Where(l => l.OtherId == userId && l.Type == UserLink.UserLinkType.Block).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new UserSilhouette(u.Id, u.Name)).
            ToListAsync());
        }
    }
}
