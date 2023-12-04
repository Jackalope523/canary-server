using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
	internal class ProfileDirector : AbstractDirector, IProfileOperations
	{
		#region Initialisation

		public ProfileDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<UserProfile> GetUserProfileAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Fail if user is blocked
            Fail(await targetUser.IsBlocking(user),
                new InvalidUserException("User is unable to view target."));

            return targetUser.ToUserProfile();
        }

        public async Task<(List<EventThinSlice> Events, List<Etching> Etchings)> GetUserNestAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Fail if user is blocked
            Fail(await user.IsBlockedBy(targetUser),
                new InvalidUserException("User is unable to view target."));

            (List<EventThinSlice> Events, List<Etching> Etchings) nest = (new(), new());

            // Check if users are friends
            if (await targetUser.IsFriendsWith(user))
            {
                // Gather active and upcoming events visible to the user
                var upcomingActivity = await GetUserActivity(targetUser);
                await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, upcomingActivity);

                // Get private events and etchings
                await targetUser.SyncPastEvents();
                nest.Events = targetUser.PastEvents.ConvertAll(e => e.ToEventThinSlice());
                nest.Events.AddRange(upcomingActivity.ConvertAll(e => new Event(e).ToEventThinSlice()));

                nest.Etchings = await Etchings.GetEtchingsByUserAsync(targetUser.Id);
            }
            else
            {
                // Get public hosted events
                nest.Events = (await Events.FindEventsByUserAsync(user.Id)).ConvertAll(e => new Event(e).ToEventThinSlice());
            }

            return nest;
        }

        public async Task<List<EventShard>> GetUserActivityAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Verify users are friends
            Try(await targetUser.IsFriendsWith(user),
                new InvalidUserException("User is unable to view target."));

            // Gather active and upcoming events
            var upcomingActivity = await GetUserActivity(targetUser);

            // Remove active and upcoming events if the user cannot view them
            await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, upcomingActivity);

            return upcomingActivity.ToList();
        }

        public async Task<IDictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(ulong userId)
        {
            var user = await GetUserAsync(userId);
            await user.SyncFriends();

            ConcurrentDictionary<UserSilhouette, List<EventShard>> friendEvents = new();

            // Gather visible activity of each friend
            user.Friends.AsParallel()
                .ForAll(async friend =>
                {
                    var friendActivity = await GetUserActivity(friend);
                    await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, friendActivity);
                    friendEvents.TryAdd(friend.ToUserSilhouette(), friendActivity);
                });

            return friendEvents;
        }

        public async Task<List<UserSilhouette>> GetFriendsAsync(ulong userId)
        {
            return await Profiles.GetFriendsAsync(userId);
        }

        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(ulong userId)
        {
            return await Profiles.GetFollowedUsersAsync(userId);
        }

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId)
        {
            return await Profiles.GetBlockedUsersAsync(userId);
        }

        public async Task FollowUserAsync(ulong userId, ulong targetId)
        {
            await Profiles.FollowUserAsync(userId, targetId);
        }

        public async Task UnfollowUserAsync(ulong userId, ulong targetId)
        {
            await Profiles.UnfollowUserAsync(userId, targetId);
        }

        public async Task BlockUserAsync(ulong userId, ulong targetId)
        {
            await Profiles.BlockUserAsync(userId, targetId);
        }

        public async Task UnblockUserAsync(ulong userId, ulong targetId)
        {
            await Profiles.UnblockUserAsync(userId, targetId);
        }

        public async Task RateUserAsync(ulong userId, ulong targetId, UserRating rating)
        {
            // Check if rating is to remove
            if (rating != UserRating.Remove)
            {
                _ = Profiles.RateUserAsync(userId, targetId, rating);
            }
            else
            {
                _ = Profiles.RemoveUserRatingAsync(userId, targetId);
            }

            var targetUser = await GetUserAsync(targetId);
            await targetUser.SyncReputation();
            targetUser.CalculateReputation();
            _ = Accounts.UpdateUserAsync(targetId, new() { (nameof(UserShard.Reputation), targetUser.Reputation) });
        }

		#endregion

		#region Favours

        internal async Task<List<User>> RequestFollowersAsync(User user)
        {
            return (await Profiles.GetUsersFollowingAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

		internal async Task<List<User>> RequestUsersBlockingAsync(User user)
        {
            return (await Profiles.GetUsersBlockingAsync(user.Id))
				.ConvertAll(user => new User(user));
        }

        internal async Task<(int Positive, int Negative)> RequestAllRatingsAsync(User user)
        {
            return await Profiles.GetUserRatingsAsync(user.Id);
        }

		#endregion

		#region Tools

		private async Task<List<EventShard>> GetUserActivity(User user)
        {
            var upcomingEventsSync = user.SyncUpcomingEvents();
            var currentEventSync = user.SyncCurrentEvent();
            
            // Gather all user event data
            await upcomingEventsSync;
            var upcomingActivity = user.UpcomingEvents;

            await currentEventSync;
            upcomingActivity.Add(user.CurrentEvent);

            return upcomingActivity
				.ConvertAll(@event => @event.ToEventShard());
        }

		#endregion
	}
}

