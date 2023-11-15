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

        public async Task<UserProfile> GetUserProfileAsync(ulong userId, ulong targetId)
        {
            var user = await GetUser(userId);
            var targetUser = await GetUser(targetId);

            // Check if user is blocked
            if (await targetUser.IsBlocking(user))
            { throw new InvalidUserException("User is unable to view target."); }

            return targetUser.ToThinProfile();
        }

        public async Task<List<EventShard>> GetUserActivityAsync(ulong userId, ulong targetId)
        {
            var user = await GetUser(userId);
            var targetUser = await GetUser(targetId);

            // Check if users are friends
            if (!await targetUser.IsFriendsWith(user))
            { throw new InvalidUserException("User is unable to view target."); }

            // Gather active and upcoming events
            var upcomingActivity = await GetUserActivityInternalAsync(targetId);

            // Remove active and upcoming events if the user cannot view them
            await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, upcomingActivity);

            return upcomingActivity.ToList();
        }

        public async Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(ulong userId)
        {
            var user = await GetUser(userId);
            var friends = Profiles.GetFriends(userId);

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

        public async Task<List<UserSilhouette>> GetFriendsAsync(ulong userId)
        {
            return Profiles.GetFriends(userId);
        }

        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userId)
        {
            return Profiles.GetFollowedUsers(userId);
        }

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId)
        {
            return Profiles.GetBlockedUsers(userId);
        }

        public async Task FollowUserAsync(ulong userId, ulong targetId)
        {
            Profiles.FollowUser(userId, targetId);
        }

        public async Task UnfollowUserAsync(ulong userId, ulong targetId)
        {
            Profiles.UnfollowUser(userId, targetId);
        }

        public async Task BlockUserAsync(ulong userId, ulong targetId)
        {
            Profiles.BlockUser(userId, targetId);
        }

        public async Task UnblockUserAsync(ulong userId, ulong targetId)
        {
            Profiles.UnblockUser(userId, targetId);
        }

        public async Task RateUserAsync(ulong userId, ulong targetId, UserRating rating)
        {
            if (rating != UserRating.Remove)
            {
                Profiles.RateUser(userId, targetId, rating);
            }
            else
            {
                Profiles.RemoveUserRating(userId, targetId);
            }

            User targetUser = new(targetId);
            await targetUser.SyncReputation();
            targetUser.CalculateReputation();
            Accounts.UpdateUser(targetId, new() { (nameof(UserShard.Reputation), targetUser.Reputation) });
        }


        internal async Task<List<UserSilhouette>> GetUsersBlockingAsync(ulong userId)
        {
            return Profiles.GetUsersBlocking(userId);
        }

        internal async Task<(int Positive, int Negative)> GetAllRatingsAsync(ulong userId)
        {
            return Profiles.GetUserRatings(userId);
        }

        private async Task<List<EventShard>> GetUserActivityInternalAsync(ulong userId)
        {
            // Gather all user event data
            var upcomingActivity = Events.FindUpcomingEventsForUser(userId);
            upcomingActivity.Add(Events.FindCurrentEventForUser(userId));

            return upcomingActivity;
        }
    }
}

