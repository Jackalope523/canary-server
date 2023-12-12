using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

namespace Core.Controls
{
	internal class ProfileDirector : AbstractDirector, IProfileOperations
	{
		public ProfileDirector(CoreTerminal terminal) : base(terminal) { }

        public async Task<UserProfile> GetUserProfileAsync(Guid userID, Guid targetID)
        {
            var user = await GetUser(userID);
            var targetUser = await GetUser(targetID);

            // Check if user is blocked
            if (await targetUser.IsBlocking(user))
            { throw new InvalidUserException("User is unable to view target."); }

            return targetUser.ToThinProfile();
        }

        public async Task<List<EventShard>> GetUserActivityAsync(Guid userID, Guid targetID)
        {
            var user = await GetUser(userID);
            var targetUser = await GetUser(targetID);

            // Check if users are friends
            if (!await targetUser.IsFriendsWith(user))
            { throw new InvalidUserException("User is unable to view target."); }

            // Gather active and upcoming events
            var upcomingActivity = await GetUserActivityInternalAsync(targetID);

            // Remove active and upcoming events if the user cannot view them
            await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, upcomingActivity);

            return upcomingActivity.ToList();
        }

        public async Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(Guid userID)
        {
            var user = await GetUser(userID);
            var friends = await Profiles.GetFriendsAsync(userID);

            Dictionary<UserSilhouette, List<EventShard>> friendEvents = new();

            // Gather visible activity of each friend
            foreach (var friend in friends)
            {
                var friendActivity = await GetUserActivityInternalAsync(friend.Id);
                await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, friendActivity);
                friendEvents.Add(friend, friendActivity);
            }

            return friendEvents;
        }

        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid userID)
        {
            return await Profiles.GetFollowedUsersAsync(userID);
        }

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid userID)
        {
            return await Profiles.GetBlockedUsersAsync(userID);
        }

        public async Task FollowUserAsync(Guid userID, Guid targetID)
        {
            Profiles.FollowUserAsync(userID, targetID);
        }

        public async Task UnfollowUserAsync(Guid userID, Guid targetID)
        {
            Profiles.UnfollowUserAsync(userID, targetID);
        }

        public async Task BlockUserAsync(Guid userID, Guid targetID)
        {
            Profiles.BlockUserAsync(userID, targetID);
        }

        public async Task UnblockUserAsync(Guid userID, Guid targetID)
        {
            Profiles.UnblockUserAsync(userID, targetID);
        }

        public async Task RateUserAsync(Guid userID, Guid targetID, UserRating rating)
        {
            if (rating != UserRating.Remove)
            {
                Profiles.RateUserAsync(userID, targetID, rating);
            }
            else
            {
                Profiles.RemoveUserRatingAsync(userID, targetID);
            }

            User targetUser = new(targetID);
            await targetUser.SyncReputation();
            targetUser.CalculateReputation();
            Accounts.UpdateUserAsync(targetID, new() { ("Reputation", targetUser.Reputation) });
        }

        internal async Task<(int Positive, int Negative)> GetAllRatingsAsync(Guid userID)
        {
            return await Profiles.GetUserRatingsAsync(userID);
        }

        private async Task<List<EventShard>> GetUserActivityInternalAsync(Guid userID)
        {
            // Gather all user event data
            var upcomingActivity = await Events.FindUpcomingEventsForUserAsync(userID);
            upcomingActivity.Add(await Events.FindCurrentEventForUserAsync(userID));

            return upcomingActivity.ToList();
        }
    }
}

