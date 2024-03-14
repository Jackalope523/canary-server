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

        public async Task<List<Etching>> GenerateFeedForUserAsync(ulong id, DateTimeOffset depthCharge, List<ulong> exclusionList)
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
               Where(p => friends.Contains(p.OwnerId) && !exclusionList.Contains(p.EventId) && p.PostedAt > depthCharge && p.PostedAt < DateTimeOffset.UtcNow).
               Join(
               storeSentry.ExecuteRead(ctx => 
               ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, p.IsHidden, l.RateUps }
               ).
               Join(
               storeSentry.ExecuteRead(ctx => 
               ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (a, b) => new { a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, a.RateUps, b.RateDowns, a.IsHidden }             
               ).
               Join(
               ctx.Users,
               p => p.OwnerId,
               u => u.Id,
               (p, u) => new Etching(p.Id, p.EventId, new(u.Id, u.Name), p.PostedAt, p.PhotoURL, new(p.RateUps, p.RateDowns), p.IsHidden)
               ).ToListAsync());

            // Compile unique list if events spanned by friend posts and a list of already loaded posts. 
            List<ulong> sitesToBeExplored = new();
            List<ulong> previouslyExtractedPosts = new();
            foreach (Etching p in friendPosts)
            {
                if (!sitesToBeExplored.Contains(p.EventId)) sitesToBeExplored.Add(p.EventId);
                previouslyExtractedPosts.Add(p.Id);
            }

            // Get remaining friend posts from same events as others even if outside time range. 
            List<Etching> nettedPosts = await storeSentry.ExecuteReadAsync(ctx => ctx.Posts.Where(p => friends.Contains(p.OwnerId) && !exclusionList.Contains(p.EventId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.EventId)).
               Join(
               ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() }),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, p.IsHidden, l.RateUps }
               ).
               Join(
               ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() }),
               p => p.Id,
               l => l.PostId,
               (a, b) => new { a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, a.RateUps, b.RateDowns, a.IsHidden }
               ).
               Join(
               ctx.Users,
               p => p.OwnerId,
               u => u.Id,
               (p,u) => new Etching(p.Id, p.EventId, new(u.Id, u.Name), p.PostedAt, p.PhotoURL, new(p.RateUps, p.RateDowns), p.IsHidden)
               ).ToListAsync());

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
