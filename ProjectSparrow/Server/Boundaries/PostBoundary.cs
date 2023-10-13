using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Boundaries
{
    public record EventHeader(Guid Id, string Name, bool IsActive, DateTimeOffset LastActiveTime);

    public record EventPost(Guid Id, Guid EventId, Guid UserId,
        DateTimeOffset TimePosted, string ImageURL,
        (int Positive, int Negative) Ratings);

    public interface IPostDatabase
    {
        List<EventPost> GetPostsForEvent(Guid id);
        List<EventPost> GetPostsByUser(Guid id);
        EventPost GetPost(Guid id);
        EventPost AddPost(Guid eventId, Guid posterId,
            DateTimeOffset timePosted, string imageURL);
        bool RemovePost(Guid postId);

        bool RatePost(Guid postId, Guid voterId, UserRating rating);
        bool RemovePostRating(Guid postId, Guid voterId);

        List<EventPost> GenerateFeedForUser(Guid id, DateTimeOffset depthCharge, List<Guid> exclusionList);
    }

    public interface IPostOperations
    {
        Task<List<EventPost>> GetEventPostsAsync(Guid userID, Guid eventID);
        Task<EventPost> AddPostAsync(Guid userID, Guid eventID, string imageURL);
        Task RemovePostAsync(Guid userID, Guid postID);
        Task RatePostAsync(Guid userID, Guid postID, UserRating rating);

        Task<(int Depth, List<EventHeader> Headers, List<EventPost> Posts)> GetUserFeedAsync(Guid userID,
            int depth, List<Guid> exclusionList = null);
    }
}

