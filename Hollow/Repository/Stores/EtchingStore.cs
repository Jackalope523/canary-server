using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Mathematics;
using Shared;
using System;

namespace Repository
{
    public class EtchingStore : QueryStore, IEtchingDatabase
    {   
        public EtchingStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<Etching> AddEtchingAsync(ulong eventId, ulong posterId, DateTimeOffset timePosted, string imageURL)
        { 
            Post toAdd = new() 
            { 
                EventId = eventId, 
                OwnerId = posterId, 
                PostedAt = timePosted, 
                PhotoURL = imageURL 
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.Add(toAdd));

            string ownerName = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == posterId).
                Select(u => u.Name).
                SingleAsync());

            return new Etching ( toAdd.Id, toAdd.EventId, new UserSilhouette(toAdd.OwnerId, ownerName), toAdd.PostedAt, toAdd.PhotoURL, new(0, 0), toAdd.IsHidden );
        }

        public async Task<List<Etching>> GenerateFeedForUserAsync(ulong id, DateTimeOffset depthCharge, DateTimeOffset lastDepthCharge)
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
            List<Etching> friendPosts = await storeSentry.ExecuteReadAsync(ctx =>
              ctx.Posts.
              Where(p => friends.Contains(p.OwnerId) && p.PostedAt >= depthCharge && p.PostedAt <= lastDepthCharge).
              Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new Etching(p.Id, p.EventId, new(u.Id, u.Name), p.PostedAt, p.PhotoURL, new(-1, -1), p.IsHidden)
               ).ToListAsync());

            List<Task<int>> friendPostsPositiveRatings = new();
            List<Task<int>> friendPostNegativeRatings = new();
            List<ulong> sitesToBeExplored = new();
            List<ulong> previouslyExtractedPosts = new();
            foreach (Etching e in friendPosts)
            {
                friendPostsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => 
                    ctx.PostLinks.
                    Where(l => l.PostId == e.Id && l.Type == PostLink.PostLinkType.RateUp).
                    CountAsync()));

                friendPostNegativeRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.PostLinks.
                    Where(l => l.PostId == e.Id && l.Type == PostLink.PostLinkType.RateDown).
                    CountAsync()));

                // Compile unique list if events spanned by friend posts and a list of already loaded posts. 
                if (!sitesToBeExplored.Contains(e.EventId)) sitesToBeExplored.Add(e.EventId);
                previouslyExtractedPosts.Add(e.Id);
            }

            // Get remaining friend posts from same events as others even if outside time range.
            List<Etching> nettedPosts = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Posts.
                Where(p => friends.Contains(p.OwnerId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.EventId)).
                Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new Etching(p.Id, p.EventId, new(u.Id, u.Name), p.PostedAt, p.PhotoURL, new(-1, -1), p.IsHidden)
                ).ToListAsync());

            List<Task<int>> nettedPostsPositiveRatings = new();
            List<Task<int>> nettedPostNegativeRatings = new();
            foreach (Etching e in nettedPosts)
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

        public async Task<Etching> GetEtchingAsync(ulong id)
        {
            Task<int> ups = storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == id && l.Type == PostLink.PostLinkType.RateUp).CountAsync());
            Task<int> downs = storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == id && l.Type == PostLink.PostLinkType.RateDown).CountAsync());

            Etching etching = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.Posts.
            Where(p => p.Id == id).
            Select(p => new Etching(p.Id, p.EventId, new UserSilhouette(p.OwnerId, null), p.PostedAt, p.PhotoURL, new (0,0), p.IsHidden)).
            SingleAsync());

            Task<string> name = storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.Id == etching.User.Id).
                Select(u => u.Name).
                SingleAsync());

            return etching with { User = new UserSilhouette(etching.User.Id, await name), Ratings = new (await ups, await downs) };
        }

        public async Task<List<Etching>> GetEtchingsByUserAsync(ulong id)
        {
            List<Etching> etchings = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Posts.Where(p => p.OwnerId == id).
                 Select(a => new Etching(a.Id, a.EventId, new UserSilhouette(a.OwnerId, null), a.PostedAt, a.PhotoURL, new(0, 0), a.IsHidden)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(etchings.Count);
            List<Task<int>> negativeRatings = new(etchings.Count);
            List<Task<string>> authorNames = new(etchings.Count);
            for (int i = 0; i < etchings.Count; i++)
            {
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == etchings[i].Id && l.Type == PostLink.PostLinkType.RateUp).CountAsync()));
                negativeRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == etchings[i].Id && l.Type == PostLink.PostLinkType.RateDown).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == etchings[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            int[] downs = await Task.WhenAll(negativeRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < etchings.Count; i++)
            {
                etchings[i] = etchings[i] with { Ratings = (ups[i], downs[i]), User = new UserSilhouette(etchings[i].User.Id, names[i]) };
            }

            return etchings;
        }

        public async Task RateEtchingAsync(ulong postId, ulong voterId, UserRating rating)
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

        public async Task RemoveEtchingAsync(ulong postId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.Remove(new Post { Id = postId }));
        }

        public async Task RemoveEtchingRatingAsync(ulong postId, ulong voterId)
        {
            Func<QueryContext, Task> query = EF.CompileAsyncQuery(
                (QueryContext ctx) =>
                ctx.PostLinks.
                Where(l => l.UserId == voterId && l.PostId == postId).
                ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(query);
        }

        public async Task<List<Etching>> GetEtchingsForEventAsync(ulong id)
        {
            List<Etching> etchings = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Posts.Where(p => p.EventId == id).
                 Select(a => new Etching(a.Id, a.EventId, new UserSilhouette(a.OwnerId, null), a.PostedAt, a.PhotoURL, new(0, 0), a.IsHidden)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(etchings.Count);
            List<Task<int>> negativeRatings = new(etchings.Count);
            List<Task<string>> authorNames = new(etchings.Count);
            for (int i = 0; i < etchings.Count; i++)
            {            
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == etchings[i].Id && l.Type == PostLink.PostLinkType.RateUp).CountAsync()));
                negativeRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.PostId == etchings[i].Id && l.Type == PostLink.PostLinkType.RateDown).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == etchings[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            int[] downs = await Task.WhenAll(negativeRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < etchings.Count; i++)
            {
                etchings[i] = etchings[i] with { Ratings = (ups[i], downs[i]), User = new UserSilhouette(etchings[i].User.Id, names[i]) };
            }

            return etchings;
        }
        public async Task HideEtchingAsync(ulong etchingId)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Post p = new() { Id = etchingId, IsHidden = true };
            storeSentry.DiscussWrite(ctx => ctx.Posts.Attach(p), currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(p).Property(nameof(p.IsHidden)).IsModified = true, currentDiscussion);           
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }
    }
}
