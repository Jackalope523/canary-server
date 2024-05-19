using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Mathematics;

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
            Post toAdd = new() 
            { 
                GatheringId = gatheringId, 
                OwnerId = posterId, 
                PostedAt = timePosted
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.Add(toAdd));

            string ownerName = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == posterId).
                Select(u => u.Name).
                SingleAsync());

            return new SnapshotShard ( toAdd.Id, toAdd.GatheringId, new UserSilhouette(toAdd.OwnerId, ownerName), toAdd.PostedAt, new(0, 0), toAdd.IsHidden );
        }

        public async Task<List<SnapshotShard>> GenerateFeedForUserAsync(ulong id, DateTimeOffset depthCharge, DateTimeOffset lastDepthCharge)
        {
            // Get List of Friends.
            Task<List<ulong>> following = storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.OtherId).
                ToListAsync());

            Task<List<ulong>> followingMe = storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow).
                Select(l => l.SelfId).
                ToListAsync());

            List<ulong> friends = (await following).Intersect(await followingMe).ToList();

            // Get unseen posts by friends from certain depth.
            List<SnapshotShard> friendPosts = await storeSentry.ExecuteReadAsync(ctx =>
              ctx.Posts.
              Where(p => friends.Contains(p.OwnerId) && p.PostedAt >= depthCharge && p.PostedAt <= lastDepthCharge).
              Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new SnapshotShard(p.Id, p.GatheringId, new(u.Id, u.Name), p.PostedAt, new(-1, -1), p.IsHidden)
               ).ToListAsync());

            List<Task<int>> friendPostsPositiveRatings = new();
            List<Task<int>> friendPostNegativeRatings = new();
            List<ulong> sitesToBeExplored = new();
            List<ulong> previouslyExtractedPosts = new();
            foreach (SnapshotShard e in friendPosts)
            {
                friendPostsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => 
                    ctx.PostLinks.
                    Where(l => l.PostId == e.Id && l.Type == PostLink.PostLinkType.RateUp).
                    CountAsync()));

                friendPostNegativeRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.PostLinks.
                    Where(l => l.PostId == e.Id && l.Type == PostLink.PostLinkType.RateDown).
                    CountAsync()));

                // Compile unique list if gatherings spanned by friend posts and a list of already loaded posts. 
                if (!sitesToBeExplored.Contains(e.GatheringId)) sitesToBeExplored.Add(e.GatheringId);
                previouslyExtractedPosts.Add(e.Id);
            }

            // Get remaining friend posts from same gatherings as others even if outside time range.
            List<SnapshotShard> nettedPosts = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Posts.
                Where(p => friends.Contains(p.OwnerId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.GatheringId)).
                Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new SnapshotShard(p.Id, p.GatheringId, new(u.Id, u.Name), p.PostedAt, new(-1, -1), p.IsHidden)
                ).ToListAsync());

            List<Task<int>> nettedPostsPositiveRatings = new();
            List<Task<int>> nettedPostNegativeRatings = new();
            foreach (SnapshotShard e in nettedPosts)
            {
                nettedPostsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.PostLinks.
                    Where(l => l.PostId == e.Id && l.Type == PostLink.PostLinkType.RateUp).
                    CountAsync()));

                nettedPostNegativeRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.PostLinks.
                    Where(l => l.PostId == e.Id && l.Type == PostLink.PostLinkType.RateDown).
                    CountAsync()));
            }

            await Task.WhenAll(friendPostsPositiveRatings);
            await Task.WhenAll(friendPostNegativeRatings);
            await Task.WhenAll(nettedPostsPositiveRatings);
            await Task.WhenAll(nettedPostNegativeRatings);

            return friendPosts.Concat(nettedPosts).ToList();
        }

        public async Task<SnapshotShard> GetSnapshotAsync(ulong id)
        {
            Task<int> ups = storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == id && l.Type == PostLink.PostLinkType.RateUp).CountAsync());
            Task<int> downs = storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == id && l.Type == PostLink.PostLinkType.RateDown).CountAsync());

            SnapshotShard snapshot = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.Posts.
            Where(p => p.Id == id).
            Select(p => new SnapshotShard(p.Id, p.GatheringId, new UserSilhouette(p.OwnerId, null), p.PostedAt, new (0,0), p.IsHidden)).
            SingleAsync());

            Task<string> name = storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.Id == snapshot.User.Id).
                Select(u => u.Name).
                SingleAsync());

            return snapshot with { User = new UserSilhouette(snapshot.User.Id, await name), Ratings = new (await ups, await downs) };
        }

        public async Task<List<SnapshotShard>> GetSnapshotsByUserAsync(ulong id)
        {
            List<SnapshotShard> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Posts.Where(p => p.OwnerId == id).
                 Select(a => new SnapshotShard(a.Id, a.GatheringId, new UserSilhouette(a.OwnerId, null), a.PostedAt, new(0, 0), a.IsHidden)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(snapshots.Count);
            List<Task<int>> negativeRatings = new(snapshots.Count);
            List<Task<string>> authorNames = new(snapshots.Count);
            for (int i = 0; i < snapshots.Count; i++)
            {
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == PostLink.PostLinkType.RateUp).CountAsync()));
                negativeRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == PostLink.PostLinkType.RateDown).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == snapshots[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            int[] downs = await Task.WhenAll(negativeRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < snapshots.Count; i++)
            {
                snapshots[i] = snapshots[i] with { Ratings = (ups[i], downs[i]), User = new UserSilhouette(snapshots[i].User.Id, names[i]) };
            }

            return snapshots;
        }

        public async Task RateSnapshotAsync(ulong postId, ulong voterId, UserRating rating)
        {
            PostLink.PostLinkType type;
            if (rating.Equals(UserRating.Positive)) type = PostLink.PostLinkType.RateUp;
            else type = PostLink.PostLinkType.RateDown;

            PostLink toAdd = new()
            {
                UserId = voterId,
                PostId = postId,
                Time = DateTimeOffset.UtcNow,
                Type = type
            };

            ulong id = await storeSentry.ExecuteReadAsync(ctx =>
                        ctx.PostLinks.
                        Where(l => l.UserId == voterId && l.PostId == postId).
                        Select(l => l.Id).
                        SingleOrDefaultAsync());

            if (id != 0)
            {
                toAdd.Id = id;
            }

            await storeSentry.ExecuteWriteAsync(ctx => ctx.PostLinks.Update(toAdd));
        }

        public async Task RemoveSnapshotAsync(ulong postId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.Remove(new Post { Id = postId }));
        }

        public async Task RemoveSnapshotRatingAsync(ulong postId, ulong voterId)
        {
            Func<QueryContext, Task> query = EF.CompileAsyncQuery(
                (QueryContext ctx) =>
                ctx.PostLinks.
                Where(l => l.UserId == voterId && l.PostId == postId).
                ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(query);
        }

        public async Task<List<SnapshotShard>> GetSnapshotsForGatheringAsync(ulong id)
        {
            List<SnapshotShard> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Posts.Where(p => p.GatheringId == id).
                 Select(a => new SnapshotShard(a.Id, a.GatheringId, new UserSilhouette(a.OwnerId, null), a.PostedAt, new(0, 0), a.IsHidden)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(snapshots.Count);
            List<Task<int>> negativeRatings = new(snapshots.Count);
            List<Task<string>> authorNames = new(snapshots.Count);
            for (int i = 0; i < snapshots.Count; i++)
            {            
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == PostLink.PostLinkType.RateUp).CountAsync()));
                negativeRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == snapshots[i].Id && l.Type == PostLink.PostLinkType.RateDown).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == snapshots[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            int[] downs = await Task.WhenAll(negativeRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < snapshots.Count; i++)
            {
                snapshots[i] = snapshots[i] with { Ratings = (ups[i], downs[i]), User = new UserSilhouette(snapshots[i].User.Id, names[i]) };
            }

            return snapshots;
        }
        public async Task HideSnapshotAsync(ulong snapshotId)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Post p = new() { Id = snapshotId, IsHidden = true };
            storeSentry.DiscussWrite(ctx => ctx.Posts.Attach(p), currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(p).Property(nameof(p.IsHidden)).IsModified = true, currentDiscussion);           
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }
    }
}
