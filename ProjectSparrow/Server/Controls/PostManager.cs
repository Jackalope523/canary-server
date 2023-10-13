using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Server.Entities;
using Shared;

namespace Server.Controls
{
	internal class PostManager : AbstractManager, IPostOperations
	{
		public PostManager(CoreTerminal terminal) : base(terminal) { }

        public async Task<List<EventPost>> GetEventPostsAsync(Guid userID, Guid eventID)
        {
            var user = await GetUser(userID);
            Event targetEvent = new(eventID);

            // Ensure user can see the event
            if (!await targetEvent.IsAttendedBy(user))
            { throw new InvalidEventException("User did not attend or is not attending event."); }

            var eventPosts = Posts.GetPostsForEvent(eventID);

            return eventPosts;
        }

        public async Task<EventPost> AddPostAsync(Guid userID, Guid eventID, string imageURL)
        {
            User user = new(userID);
            var targetEvent = await GetEvent(eventID);

            // Ensure the user can post to the event
            if (!await targetEvent.IsAttendedBy(user))
            { throw new InvalidEventException("User is not attending event."); }

            // Ensure event is still running
            if (targetEvent.EndTime.HasValue)
            { throw new InvalidEventException("Event has already ended."); }

            // Try to post
            var userPost = Posts.AddPost(eventID, userID, DateTimeOffset.UtcNow, imageURL);

            return userPost;
        }

        public async Task RemovePostAsync(Guid userID, Guid postID)
        {
            var eventPost = Posts.GetPost(postID);

            // Check if user can delete post
            if (!eventPost.UserId.Equals(userID))
            { throw new InvalidUserException("User cannot remove post."); }

            Posts.RemovePost(postID);
        }

        public async Task RatePostAsync(Guid userID, Guid postID, UserRating rating)
        {
            User user = new(userID);
            var eventOfPost = await GetEvent(Posts.GetPost(postID).EventId);

            // Check if user can interact with post
            if (!await eventOfPost.IsAttendedBy(user))
            { throw new InvalidUserException("User cannot interact with post."); }

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                Posts.RatePost(userID, postID, rating);
            }
            else
            {
                Posts.RemovePostRating(postID, userID);
            }
        }

        public async Task<(int Depth, List<EventHeader> Headers, List<EventPost> Posts)>
            GetUserFeedAsync(Guid userID, int depth = 0, List<Guid> exclusionList = null)
        {
            User user = new(userID);
            exclusionList ??= new();
            Dictionary<Guid, EventHeader> eventHeaders = new();

            // Retrieve friend-populated event posts after a specified time excluding previously viewed events
            DateTimeOffset depthCharge = DateTimeOffset.UtcNow - TimeSpan.FromDays(1 + depth);
            var friendPosts = Posts.GenerateFeedForUser(user.Id, depthCharge, exclusionList);

            // Get the respective event headers for the posts
            foreach (EventPost post in friendPosts)
            {
                // Add event header if it does not yet exist
                if (!eventHeaders.ContainsKey(post.EventId))
                {
                    Event postEvent = new(Events.FindEvent(post.EventId));

                    eventHeaders.Add(post.EventId, postEvent.ToEventHeader(post.TimePosted));
                }
                // Update event header active time if post is more recent
                else if (eventHeaders[post.EventId].LastActiveTime < post.TimePosted)
                {
                    eventHeaders[post.EventId] = new(post.EventId,
                        eventHeaders[post.EventId].Name,
                        eventHeaders[post.EventId].IsActive,
                        post.TimePosted);
                }
            }

            return (depth, eventHeaders.Values.ToList(), friendPosts);
        }


        internal async Task<List<EventPost>> GetEventPostsAsync(Guid eventID)
        {
            return Posts.GetPostsForEvent(eventID);
        }
    }
}

