using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Server.Entities;
using Shared;

namespace Server.Controls
{
	internal class ProfileManager : AbstractManager, IProfileOperations
	{
		public ProfileManager(CoreTerminal terminal) : base(terminal) { }

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
            await Terminal.EventManager.RemoveInaccessibleEventsAsync(user, upcomingActivity);

            return upcomingActivity.ToList();
        }

        public async Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(Guid userID)
        {
            var user = await GetUser(userID);
            var friends = Profiles.GetFriends(userID);

            Dictionary<UserSilhouette, List<EventShard>> friendEvents = new();

            // Gather visible activity of each friend
            foreach (var friend in friends)
            {
                var friendActivity = await GetUserActivityInternalAsync(friend.Id);
                await Terminal.EventManager.RemoveInaccessibleEventsAsync(user, friendActivity);
                friendEvents.Add(friend, friendActivity);
            }

            return friendEvents;
        }

        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid userID)
        {
            return Profiles.GetFollowedUsers(userID);
        }

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid userID)
        {
            return Profiles.GetBlockedUsers(userID);
        }

        public async Task FollowUserAsync(Guid userID, Guid targetID)
        {
            Profiles.FollowUser(userID, targetID);
        }

        public async Task UnfollowUserAsync(Guid userID, Guid targetID)
        {
            Profiles.UnfollowUser(userID, targetID);
        }

        public async Task BlockUserAsync(Guid userID, Guid targetID)
        {
            Profiles.BlockUser(userID, targetID);
        }

        public async Task UnblockUserAsync(Guid userID, Guid targetID)
        {
            Profiles.UnblockUser(userID, targetID);
        }

        public async Task RateUserAsync(Guid userID, Guid targetID, UserRating rating)
        {
            if (rating != UserRating.Remove)
            {
                Profiles.RateUser(userID, targetID, rating);
            }
            else
            {
                Profiles.RemoveUserRating(userID, targetID);
            }

            User targetUser = new(targetID);
            await targetUser.SyncReputation();
            targetUser.CalculateReputation();
            Accounts.UpdateUser(targetID, new() { ("Reputation", targetUser.Reputation) });
        }

        internal async Task<(int Positive, int Negative)> GetAllRatingsAsync(Guid userID)
        {
            return Profiles.GetUserRatings(userID);
        }

        private async Task<List<EventShard>> GetUserActivityInternalAsync(Guid userID)
        {
            // Gather all user event data
            var upcomingActivity = Events.FindUpcomingEventsForUser(userID);
            upcomingActivity.Add(Events.FindCurrentEventForUser(userID));

            return upcomingActivity.ToList();
        }
    }
}

