using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository
{
    public class EtchingStore : QueryStore, IEtchingDatabase
    {
        public static IEtchingDatabase EtchingDatabaseAccess => new EtchingStore(new TestSentry());

        public EtchingStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<Etching> AddEtchingAsync(Guid eventId, Guid posterId, DateTimeOffset timePosted, string imageURL)
        {
            Post toAdd = new Post { EventId = eventId, OwnerId = posterId, PostedAt = timePosted, PhotoURL = imageURL };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.Add(toAdd));
            return new Etching(toAdd.Id, toAdd.EventId, toAdd.OwnerId, toAdd.PostedAt, toAdd.PhotoURL, new(0, 0));
        }

        public async Task<List<Etching>> GenerateFeedForUserAsync(Guid id, DateTimeOffset depthCharge, List<Guid> exclusionList)
        {
            // Get List of Friends.
            Task<List<Guid>> following = storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.OtherId).ToListAsync());
            Task<List<Guid>> followingMe = storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.SelfId).ToListAsync());

            List <Guid> a = await following;
            List<Guid> b = await followingMe;
            List<Guid> friends = a.Intersect(b).ToList();

            // Get unseen posts by friends from certain depth.
            List<Etching> friendPosts = await storeSentry.ExecuteReadAsync(ctx => ctx.Posts.Where(p => friends.Contains(p.OwnerId) && !exclusionList.Contains(p.EventId) && p.PostedAt > depthCharge && p.PostedAt < DateTimeOffset.UtcNow).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
               ).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
               )).ToListAsync());

            // Compile unique list if events spanned by friend posts and a list of already loaded posts. 
            List<Guid> sitesToBeExplored = new List<Guid>();
            List<Guid> previouslyExtractedPosts = new List<Guid>();
            foreach (Etching p in friendPosts)
            {
                if (!sitesToBeExplored.Contains(p.EventId)) sitesToBeExplored.Add(p.EventId);
                previouslyExtractedPosts.Add(p.Id);
            }

            // Get remaining friend posts from same events as others even if outside time range. 
            List<Etching> nettedPosts = await storeSentry.ExecuteReadAsync(ctx => ctx.Posts.Where(p => friends.Contains(p.OwnerId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.EventId)).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
               ).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
               )).ToListAsync());

            return friendPosts.Concat(nettedPosts).ToList();
        }

        public async Task<Etching> GetEtchingAsync(Guid id)
        {
            Task<int> Ups = CountRatingsAsync(id, PostLink.PostLinkType.RateUp);
            Task<int> Downs = CountRatingsAsync(id, PostLink.PostLinkType.RateDown);
            int u = await Ups;
            int d = await Downs;

            return await storeSentry.ExecuteReadAsync(ctx => ctx.
                Posts.
                Where(p => p.Id == id).
                Select(p => new Etching(p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, new(u, d))).SingleAsync());
        }

        public async Task<List<Etching>> GetEtchingsByUserAsync(Guid id)
        {

            return await storeSentry.ExecuteReadAsync(ctx => ctx.Posts.Where(p => p.OwnerId == id).
                Join(
                storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
                p => p.Id,
                l => l.PostId,
                (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
                ).
                Join(
                storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
                p => p.Id,
                l => l.PostId,
                (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
                )).ToListAsync());
        }

        public async Task<bool> RateEtchingAsync(Guid postId, Guid voterId, UserRating rating)
        {
            PostLink.PostLinkType type;
            if (rating.Equals(UserRating.Positive)) type = PostLink.PostLinkType.RateUp;
            else type = PostLink.PostLinkType.RateDown;

            return await AddLinkOperationAsync(new PostLink { SelfId = voterId, OtherId = postId, Type = type });
        }

        public async Task<bool> RemoveEtchingAsync(Guid postId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Posts.Remove(new Post { Id = postId }));
            return true;
        }

        public async Task<bool> RemoveEtchingRatingAsync(Guid postId, Guid voterId)
        {
            return await RemoveLinkOperationAsync(new PostLink { SelfId = voterId, OtherId = postId });
        }

        public async Task<List<Etching>> GetEtchingsForEventAsync(Guid id)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.Posts.Where(p => p.EventId == id).
                Join(
                storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
                p => p.Id,
                l => l.PostId,
                (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
                ).
                Join(
                storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.OtherId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
                p => p.Id,
                l => l.PostId,
                (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
                )).ToListAsync());
        }
    }


}
