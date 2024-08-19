using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Mathematics;
using Serilog;
using Serilog.Core;
using System;

namespace Repository
{
    public class EFCoreSnapshotStore : QueryStore, ISnapshotDatabase
    {   
        public EFCoreSnapshotStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task<SnapshotShard> AddSnapshotAsync(ulong gatheringId, ulong posterId, DateTimeOffset timePosted)
        { 
            Snapshot toAdd = new() 
            { 
                GatheringId = gatheringId, 
                OwnerId = posterId, 
                PostedAt = timePosted
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Snapshots.Add(toAdd));

            string ownerName = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == posterId).
                Select(u => u.Name).
                SingleAsync());

            return new SnapshotShard ( toAdd.Id, toAdd.GatheringId, new UserShard(toAdd.OwnerId, ownerName), toAdd.PostedAt, 0);
        }

        public async Task<List<SnapshotShard>> GenerateColumnForUserAsync(ulong id, DateTimeOffset depthCharge, DateTimeOffset lastDepthCharge)
        {
            // Get List of Companions.
            Task<List<ulong>> appreciating = storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Appreciate).Select(l => l.OtherId).
                ToListAsync());

            Task<List<ulong>> appreciatingMe = storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Appreciate).
                Select(l => l.SelfId).
                ToListAsync());

            List<ulong> companions = (await appreciating).Intersect(await appreciatingMe).ToList();

            // Get unseen posts by companions from certain depth.
            List<SnapshotShard> companionPosts = await storeSentry.ExecuteReadAsync(ctx =>
              ctx.Snapshots.
              Where(p => companions.Contains(p.OwnerId) && p.PostedAt >= depthCharge && p.PostedAt <= lastDepthCharge).
              Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new SnapshotShard(p.Id, p.GatheringId, new(u.Id, u.Name), p.PostedAt, -1)
               ).ToListAsync());

            List<Task<int>> companionPostsPositiveRatings = new();
            List<Task<int>> companionPostNegativeRatings = new();
            List<ulong> sitesToBeExplored = new();
            List<ulong> previouslyExtractedPosts = new();
            foreach (SnapshotShard e in companionPosts)
            {
                companionPostsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => 
                    ctx.SnapshotLinks.
                    Where(l => l.PostId == e.Id && l.Type == SnapshotLink.SnapshotLinkType.RateUp).
                    CountAsync()));

                companionPostNegativeRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.SnapshotLinks.
                    Where(l => l.PostId == e.Id && l.Type == SnapshotLink.SnapshotLinkType.RateDown).
                    CountAsync()));

                // Compile unique list if gatherings spanned by companion posts and a list of already loaded posts. 
                if (!sitesToBeExplored.Contains(e.GatheringId)) sitesToBeExplored.Add(e.GatheringId);
                previouslyExtractedPosts.Add(e.Id);
            }

            // Get remaining companion posts from same gatherings as others even if outside time range.
            List<SnapshotShard> nettedPosts = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Snapshots.
                Where(p => companions.Contains(p.OwnerId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.GatheringId)).
                Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new SnapshotShard(p.Id, p.GatheringId, new(u.Id, u.Name), p.PostedAt, -1)
                ).ToListAsync());

            List<Task<int>> nettedPostsPositiveRatings = new();
            List<Task<int>> nettedPostNegativeRatings = new();
            foreach (SnapshotShard e in nettedPosts)
            {
                nettedPostsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.SnapshotLinks.
                    Where(l => l.PostId == e.Id && l.Type == SnapshotLink.SnapshotLinkType.RateUp).
                    CountAsync()));

                nettedPostNegativeRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.SnapshotLinks.
                    Where(l => l.PostId == e.Id && l.Type == SnapshotLink.SnapshotLinkType.RateDown).
                    CountAsync()));
            }

            await Task.WhenAll(companionPostsPositiveRatings);
            await Task.WhenAll(companionPostNegativeRatings);
            await Task.WhenAll(nettedPostsPositiveRatings);
            await Task.WhenAll(nettedPostNegativeRatings);

            return companionPosts.Concat(nettedPosts).ToList();
        }

        public async Task<SnapshotShard> GetSnapshotAsync(ulong id)
        {
            Task<int> ups = storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.PostId == id && l.Type == SnapshotLink.SnapshotLinkType.RateUp).CountAsync());

            SnapshotShard snapshot = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.Snapshots.
            Where(p => p.Id == id).
            Select(p => new SnapshotShard(p.Id, p.GatheringId, new UserShard(p.OwnerId, null), p.PostedAt, 0)).
            SingleAsync());

            Task<string> name = storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.Id == snapshot.User.Id).
                Select(u => u.Name).
                SingleAsync());

            return snapshot with { User = new UserShard(snapshot.User.Id, await name), Acclaim = await ups };
        }

        public async Task<List<SnapshotShard>> GetSnapshotsByUserAsync(ulong id)
        {
            List<SnapshotShard> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Snapshots.Where(p => p.OwnerId == id).
                 Select(a => new SnapshotShard(a.Id, a.GatheringId, new UserShard(a.OwnerId, null), a.PostedAt, 0)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(snapshots.Count);
            List<Task<int>> negativeRatings = new(snapshots.Count);
            List<Task<string>> authorNames = new(snapshots.Count);
            for (int i = 0; i < snapshots.Count; i++)
            {
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == SnapshotLink.SnapshotLinkType.RateUp).CountAsync()));
                negativeRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == SnapshotLink.SnapshotLinkType.RateDown).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == snapshots[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            int[] downs = await Task.WhenAll(negativeRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < snapshots.Count; i++)
            {
                snapshots[i] = snapshots[i] with { Acclaim = ups[i], User = new UserShard(snapshots[i].User.Id, names[i]) };
            }

            return snapshots;
        }

        public async Task AcclaimSnapshotAsync(ulong postId, ulong voterId)
        {
            SnapshotLink toAdd = new()
            {
                UserId = voterId,
                PostId = postId,
                Time = DateTimeOffset.UtcNow,
                Type = SnapshotLink.SnapshotLinkType.RateUp
            };

            ulong id = await storeSentry.ExecuteReadAsync(ctx =>
                        ctx.SnapshotLinks.
                        Where(l => l.UserId == voterId && l.PostId == postId).
                        Select(l => l.Id).
                        SingleOrDefaultAsync());

            if (id != 0)
            {
                toAdd.Id = id;
            }

            await storeSentry.ExecuteWriteAsync(ctx => ctx.SnapshotLinks.Update(toAdd));
        }

        public async Task RemoveSnapshotAsync(ulong postId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Snapshots.Remove(new Snapshot { Id = postId }));
        }

        public async Task RemoveSnapshotAcclaimAsync(ulong postId, ulong voterId)
        {
            Func<QueryContext, Task> query = EF.CompileAsyncQuery(
                (QueryContext ctx) =>
                ctx.SnapshotLinks.
                Where(l => l.UserId == voterId && l.PostId == postId).
                ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(query);
        }

        public async Task<List<SnapshotShard>> GetSnapshotsForGatheringAsync(ulong id)
        {
            List<SnapshotShard> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Snapshots.Where(p => p.GatheringId == id).
                 Select(a => new SnapshotShard(a.Id, a.GatheringId, new UserShard(a.OwnerId, null), a.PostedAt, 0)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(snapshots.Count);
            List<Task<int>> negativeRatings = new(snapshots.Count);
            List<Task<string>> authorNames = new(snapshots.Count);
            for (int i = 0; i < snapshots.Count; i++)
            {            
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == SnapshotLink.SnapshotLinkType.RateUp).CountAsync()));
                negativeRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == SnapshotLink.SnapshotLinkType.RateDown).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == snapshots[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            int[] downs = await Task.WhenAll(negativeRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < snapshots.Count; i++)
            {
                snapshots[i] = snapshots[i] with { Acclaim = ups[i], User = new UserShard(snapshots[i].User.Id, names[i]) };
            }

            return snapshots;
        }
    }
}
