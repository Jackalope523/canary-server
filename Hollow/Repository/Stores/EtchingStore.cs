using Core.Boundaries;
using Shared;

namespace Repository
{
    internal class EtchingStore : QueryStore, IEtchingDatabase
    {
        public static IEtchingDatabase EtchingDatabaseAccess => new EtchingStore(new TestSentry());

        public EtchingStore(Sentry sentry) : base(sentry)
        {
        }

        public Etching AddEtching(Guid eventId, Guid posterId, DateTimeOffset timePosted, string imageURL)
        {
            Post toAdd = new Post { EventId = eventId, OwnerId = posterId, PostedAt = timePosted, PhotoURL = imageURL };
            storeSentry.ExecuteWrite(ctx => ctx.Posts.Add(toAdd));
            return new Etching(toAdd.Id, toAdd.EventId, toAdd.OwnerId, toAdd.PostedAt, toAdd.PhotoURL, new(0, 0));
        }

        public List<Etching> GenerateFeedForUser(Guid id, DateTimeOffset depthCharge, List<Guid> exclusionList)
        {
            // Get List of Friends.
            List<Guid> following = storeSentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.OtherId).ToList());
            List<Guid> followingMe = storeSentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.SelfId).ToList());
            List<Guid> friends = following.Intersect(followingMe).ToList();

            // Get unseen posts by friends from certain depth.
            List<Etching> friendPosts = storeSentry.ExecuteRead(ctx => ctx.Posts.Where(p => friends.Contains(p.OwnerId) && !exclusionList.Contains(p.EventId) && p.PostedAt > depthCharge && p.PostedAt < DateTimeOffset.UtcNow).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
               ).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
               )).ToList());

            // Compile unique list if events spanned by friend posts and a list of already loaded posts. 
            List<Guid> sitesToBeExplored = new List<Guid>();
            List<Guid> previouslyExtractedPosts = new List<Guid>();
            foreach (Etching p in friendPosts)
            {
                if (!sitesToBeExplored.Contains(p.EventId)) sitesToBeExplored.Add(p.EventId);
                previouslyExtractedPosts.Add(p.Id);
            }

            // Get remaining friend posts from same events as others even if outside time range. 
            List<Etching> nettedPosts = storeSentry.ExecuteRead(ctx => ctx.Posts.Where(p => friends.Contains(p.OwnerId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.EventId)).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
               ).
               Join(
               storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
               p => p.Id,
               l => l.PostId,
               (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
               )).ToList());

            return friendPosts.Concat(nettedPosts).ToList();
        }

        public Etching GetEtching(Guid id)
        {
            int Ups = countRatings(id, PostLink.PostLinkType.RateUp);
            int Downs = countRatings(id, PostLink.PostLinkType.RateDown);

            return storeSentry.ExecuteRead(ctx => ctx.
                Posts.
                Where(p => p.Id == id).
                Select(p => new Etching(p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, new(Ups, Downs))).Single());
        }

        public List<Etching> GetEtchingsByUser(Guid id)
        {

            return storeSentry.ExecuteRead(ctx => ctx.Posts.Where(p => p.OwnerId == id).
                Join(
                storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() })),
                p => p.Id,
                l => l.PostId,
                (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
                ).
                Join(
                storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() })),
                p => p.Id,
                l => l.PostId,
                (a, b) => new Etching(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
                )).ToList());
        }

        public bool RateEtching(Guid postId, Guid voterId, UserRating rating)
        {
            PostLink.PostLinkType type;
            if (rating.Equals(UserRating.Positive)) type = PostLink.PostLinkType.RateUp;
            else type = PostLink.PostLinkType.RateDown;

            return addLinkOperation(new PostLink { SelfId = voterId, PostId = postId, Type = type });
        }

        public bool RemoveEtching(Guid postId)
        {
            storeSentry.ExecuteWrite(ctx => ctx.Posts.Remove(new Post { Id = postId }));
            return true;
        }

        public bool RemoveEtchingRating(Guid postId, Guid voterId)
        {
            return removeLinkOperation(new PostLink { SelfId = voterId, PostId = postId });
        }

        public List<Etching> GetEtchingsForEvent(Guid id)
        {
            throw new NotImplementedException();
        }
    }


}
