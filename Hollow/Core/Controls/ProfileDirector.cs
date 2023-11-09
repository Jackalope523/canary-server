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

        public async Task<UserProfile> GetUserProfileAsync(ulong userID, ulong targetID)
        {
            var user = await GetUser(userID);
            var targetUser = await GetUser(targetID);

            // Check if user is blocked
            if (await targetUser.IsBlocking(user))
            { throw new InvalidUserException("User is unable to view target."); }

            return targetUser.ToThinProfile();
        }

        public async Task<List<EventShard>> GetUserActivityAsync(ulong userID, ulong targetID)
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

        public async Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(ulong userID)
        {
            var user = await GetUser(userID);
            var friends = Profiles.GetFriends(userID);

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

        public async Task<List<UserSilhouette>> GetFriendsAsync(ulong userID)
        {
            return Profiles.GetFriends(userID);
        }

        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userID)
        {
            return Profiles.GetFollowedUsers(userID);
        }

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userID)
        {
            return Profiles.GetBlockedUsers(userID);
        }

        public async Task FollowUserAsync(ulong userID, ulong targetID)
        {
            Profiles.FollowUser(userID, targetID);
        }

        public async Task UnfollowUserAsync(ulong userID, ulong targetID)
        {
            Profiles.UnfollowUser(userID, targetID);
        }

        public async Task BlockUserAsync(ulong userID, ulong targetID)
        {
            Profiles.BlockUser(userID, targetID);
        }

        public async Task UnblockUserAsync(ulong userID, ulong targetID)
        {
            Profiles.UnblockUser(userID, targetID);
        }

        public async Task RateUserAsync(ulong userID, ulong targetID, UserRating rating)
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
            Accounts.UpdateUser(targetID, new() { (nameof(UserShard.Reputation), targetUser.Reputation) });
        }


        internal async Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userID)
        {
            return Profiles.GetUsersBlocking(userID);
        }

        internal async Task<(int Positive, int Negative)> GetAllRatingsAsync(ulong userID)
        {
            return Profiles.GetUserRatings(userID);
        }

        private async Task<List<EventShard>> GetUserActivityInternalAsync(ulong userID)
        {
            // Gather all user event data
            var upcomingActivity = Events.FindUpcomingEventsForUser(userID);
            upcomingActivity.Add(Events.FindCurrentEventForUser(userID));

            return upcomingActivity;
        }
    }
}

