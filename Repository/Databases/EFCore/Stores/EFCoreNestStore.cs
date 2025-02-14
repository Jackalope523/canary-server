using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreNestStore : QueryStore, INestDatabase
    {     
        private static readonly Func<CanaryContext, long, long, UserRelationship.UserLinkType, Task> RemoveLinkOperation =
            EF.CompileAsyncQuery(
                (CanaryContext ctx, long selfId, long otherId, UserRelationship.UserLinkType type) =>
                ctx.UserRelationships
                .Where(l => l.SelfId == selfId && l.OtherId == otherId && l.Type == type)
                .ExecuteDelete());

        public EFCoreNestStore(Harbor.Flag flag) : base(flag)
        {

        }
        
        public async Task FollowUserAsync(long selfId, long targetId, DateTimeOffset time) 
        {
            long id = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserRelationships.
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
                    Type = UserRelationship.UserLinkType.Follow
                };

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserRelationships.Add(toAdd));
            }
            else
            {
                UserRelationship toUpdate = new()
                {
                    Id = id,
                    SelfId = selfId,
                    OtherId = targetId,
                    Time = time,
                    Type = UserRelationship.UserLinkType.Follow
                };

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserRelationships.Update(toUpdate));
            }
        }
        public async Task UnfollowUserAsync(long selfId, long targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserRelationship.UserLinkType.Follow));
        }
        public async Task BlockUserAsync(long selfId, long targetId, DateTimeOffset time) 
        {
            long id = await storeSentry.ExecuteReadAsync(ctx =>
               ctx.UserRelationships.
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

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserRelationships.Add(toAdd));
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

                await storeSentry.ExecuteWriteAsync(ctx => ctx.UserRelationships.Update(toUpdate));
            }
        }
        public async Task UnblockUserAsync(long selfId, long targetId) 
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            RemoveLinkOperation(ctx, selfId, targetId, UserRelationship.UserLinkType.Block));
        }

        public async Task<List<CoreUser>> GetFollowedUsersAsync(long id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserRelationships.Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Follow).
             Join(
                 ctx.Users,
                 l => l.OtherId,
                 u => u.Id,
                 (l, u) => new CoreUser(u.Id,
                      u.PhoneNumber,
                      u.Email,
                      u.Name,
                      u.CompanionshipCode,
                      u.DateOfBirth,
                      u.IsPhoneConfirmed,
                      u.IsEmailConfirmed,
                      u.SoftDeleted,
                      u.SecurityStamp,
                      u.LockoutDate,
                      u.AccessTries,
                      u.AccountStatus,
                      u.JoinDate,
                      u.Reputation,
                      new CharacterShard(
                          u.Age,
                          u.Extroversion,
                          u.Athleticisme,
                          u.Chaos,
                          u.Competitiveness,
                          u.Industriousness,
                          u.NightOwl,
                          u.Openness),
                      u.TimeOfUserAgreement,
                      u.NotificationId
                  )
                 ).
             ToListAsync());
        }
        public async Task<List<BlockedUserShard>> GetBlockedUsersAsync(long id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.UserRelationships.Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Block).
            Join(
                ctx.Users, 
                l => l.OtherId, 
                u => u.Id, 
                (l,u) => new BlockedUserShard(u.Id, u.Name, l.Time)
                ).
            ToListAsync());
        }
        public async Task<List<CoreUser>> GetCompanionsAsync(long id)
        {
            Task<List<CoreUser>> appreciating = storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserRelationships.Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Follow).
             Join(
                 ctx.Users,
                 l => l.OtherId,
                 u => u.Id,
                 (l, u) => new CoreUser(u.Id,
                      u.PhoneNumber,
                      u.Email,
                      u.Name,
                      u.CompanionshipCode,
                      u.DateOfBirth,
                      u.IsPhoneConfirmed,
                      u.IsEmailConfirmed,
                      u.SoftDeleted,
                      u.SecurityStamp,
                      u.LockoutDate,
                      u.AccessTries,
                      u.AccountStatus,
                      u.JoinDate,
                      u.Reputation,
                      new CharacterShard(
                          u.Age,
                          u.Extroversion,
                          u.Athleticisme,
                          u.Chaos,
                          u.Competitiveness,
                          u.Industriousness,
                          u.NightOwl,
                          u.Openness),
                      u.TimeOfUserAgreement,
                      u.NotificationId
                  )
                 ).
             ToListAsync());

            Task<List<CoreUser>> appreciatingMe = storeSentry.ExecuteReadAsync(ctx =>
             ctx.UserRelationships.Where(l => l.OtherId == id && l.Type == UserRelationship.UserLinkType.Follow).
             Join(
                 ctx.Users,
                 l => l.SelfId,
                 u => u.Id,
                 (l, u) => new CoreUser(u.Id,
                      u.PhoneNumber,
                      u.Email,
                      u.Name,
                      u.CompanionshipCode,
                      u.DateOfBirth,
                      u.IsPhoneConfirmed,
                      u.IsEmailConfirmed,
                      u.SoftDeleted,
                      u.SecurityStamp,
                      u.LockoutDate,
                      u.AccessTries,
                      u.AccountStatus,
                      u.JoinDate,
                      u.Reputation,
                      new CharacterShard(
                          u.Age,
                          u.Extroversion,
                          u.Athleticisme,
                          u.Chaos,
                          u.Competitiveness,
                          u.Industriousness,
                          u.NightOwl,
                          u.Openness),
                      u.TimeOfUserAgreement,
                      u.NotificationId
                  )
                 ).
             ToListAsync());

            return (await appreciating).Intersect(await appreciatingMe).ToList();
        }

        public async Task<List<CoreUser>> GetUserFollowersAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserRelationships.Where(l => l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Follow).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new CoreUser(u.Id,
                      u.PhoneNumber,
                      u.Email,
                      u.Name,
                      u.CompanionshipCode,
                      u.DateOfBirth,
                      u.IsPhoneConfirmed,
                      u.IsEmailConfirmed,
                      u.SoftDeleted,
                      u.SecurityStamp,
                      u.LockoutDate,
                      u.AccessTries,
                      u.AccountStatus,
                      u.JoinDate,
                      u.Reputation,
                      new CharacterShard(
                          u.Age,
                          u.Extroversion,
                          u.Athleticisme,
                          u.Chaos,
                          u.Competitiveness,
                          u.Industriousness,
                          u.NightOwl,
                          u.Openness),
                      u.TimeOfUserAgreement,
                      u.NotificationId
                  )).
            ToListAsync());
        }

        public async Task<List<CoreUser>> GetUsersBlockingAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
            ctx.UserRelationships.Where(l => l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Block).
            Join(ctx.Users,
            l => l.SelfId,
            u => u.Id,
            (l, u) => new CoreUser(u.Id,
                      u.PhoneNumber,
                      u.Email,
                      u.Name,
                      u.CompanionshipCode,
                      u.DateOfBirth,
                      u.IsPhoneConfirmed,
                      u.IsEmailConfirmed,
                      u.SoftDeleted,
                      u.SecurityStamp,
                      u.LockoutDate,
                      u.AccessTries,
                      u.AccountStatus,
                      u.JoinDate,
                      u.Reputation,
                      new CharacterShard(
                          u.Age,
                          u.Extroversion,
                          u.Athleticisme,
                          u.Chaos,
                          u.Competitiveness,
                          u.Industriousness,
                          u.NightOwl,
                          u.Openness),
                      u.TimeOfUserAgreement,
                      u.NotificationId
                  )).
            ToListAsync());
        }

        public Task<bool> HaveMutualGathering(long userId, long targetId)
        {
            return storeSentry.ExecuteReadAsync(ctx => 
                ctx.GatheringLinks.
                Where(l => l.UserId == userId && l.Type == GatheringBond.Arrived).
                Join(
                      ctx.GatheringLinks.Where(l => l.UserId == targetId && l.Type == GatheringBond.Arrived),
                      x => x.GatheringId,
                      y => y.GatheringId,
                      (x,y) => x.GatheringId
                    ).
                AnyAsync());
        }

        public async Task<CoreGathering> GetFirstMutualGathering(long userId, long targetId)
        {
            List<long> a = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
                Where(l => l.UserId == userId && l.Type == GatheringBond.Arrived).
                Select(l => l.GatheringId).
                ToListAsync());

            List<long> b = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
                Where(l => l.UserId == targetId && l.Type == GatheringBond.Arrived).
                Select(l => l.GatheringId).
                ToListAsync());

            List<long> mutualGatherings = a.Intersect(b).ToList();

            return await storeSentry.ExecuteReadAsync(ctx => ctx.Gatherings.
                Where(g => mutualGatherings.Contains(g.Id)).
                OrderByDescending(g => g.StartTime).
                GroupJoin(
                ctx.Users,
                e => e.HostId,
                u => u.Id,
                (e, users) => new { e, user = users.FirstOrDefault() }).
                Select(
                combined => new CoreGathering
                (
                    combined.e.Id,
                    combined.user != null ? combined.user.Id : 0,
                    combined.e.Title,
                    combined.e.Description,
                    combined.e.StartTime,
                    combined.e.Location.Y,
                    combined.e.Location.X,
                    combined.e.FriendlyLocation,
                    combined.e.EndTime,
                    combined.e.State,
                    combined.e.GroupMinimum,
                    combined.e.GroupMaximum,
                    new CharacterShard(
                        combined.e.Age,
                        combined.e.Extroversion,
                        combined.e.Athleticisme,
                        combined.e.Chaos,
                        combined.e.Competitiveness,
                        combined.e.Industriousness,
                        combined.e.NightOwl,
                        combined.e.Openness),
                    combined.e.Radius,
                    combined.e.IsDynamic,
                    combined.e.SoftDeleted,
                    combined.e.NumberOfGuests,
                    combined.e.DegreeOfPrivacy,
                    combined.e.Visibility,
                    combined.e.TimeOfCreation,
                    combined.e.Decay
                )).FirstAsync());
        }

        public async Task<CoreGathering> GetLatestMutualGathering(long userId, long targetId)
        {
            List<long> a = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
               Where(l => l.UserId == userId && l.Type == GatheringBond.Arrived).
               Select(l => l.GatheringId).
               ToListAsync());

            List<long> b = await storeSentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.
                Where(l => l.UserId == targetId && l.Type == GatheringBond.Arrived).
                Select(l => l.GatheringId).
                ToListAsync());

            List<long> mutualGatherings = a.Intersect(b).ToList();

            return await storeSentry.ExecuteReadAsync(ctx => ctx.Gatherings.
                Where(g => mutualGatherings.Contains(g.Id)).
                OrderBy(g => g.StartTime).
                GroupJoin(
                ctx.Users,
                e => e.HostId,
                u => u.Id,
                (e, users) => new { e, user = users.FirstOrDefault() }).
                Select(
                combined => new CoreGathering
                (
                    combined.e.Id,
                    combined.user != null ? combined.user.Id : 0,
                    combined.e.Title,
                    combined.e.Description,
                    combined.e.StartTime,
                    combined.e.Location.Y,
                    combined.e.Location.X,
                    combined.e.FriendlyLocation,
                    combined.e.EndTime,
                    combined.e.State,
                    combined.e.GroupMinimum,
                    combined.e.GroupMaximum,
                    new CharacterShard(
                        combined.e.Age,
                        combined.e.Extroversion,
                        combined.e.Athleticisme,
                        combined.e.Chaos,
                        combined.e.Competitiveness,
                        combined.e.Industriousness,
                        combined.e.NightOwl,
                        combined.e.Openness),
                    combined.e.Radius,
                    combined.e.IsDynamic,
                    combined.e.SoftDeleted,
                    combined.e.NumberOfGuests,
                    combined.e.DegreeOfPrivacy,
                    combined.e.Visibility,
                    combined.e.TimeOfCreation,
                    combined.e.Decay
                )).FirstAsync());
        }

        public async Task<DateTimeOffset> BlockedSince(long userId, long targetId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
                    ctx.UserRelationships.
                    Where(l => l.SelfId == userId && l.OtherId == targetId && l.Type == UserRelationship.UserLinkType.Block).
                    Select(l => l.Time).
                    SingleAsync());
        }
    }
}
