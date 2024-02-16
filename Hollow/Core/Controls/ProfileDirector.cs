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

            // Check if user is themself
            if (user.Equals(targetUser))
            {
                // Gather active and upcoming events visible to the user
                var upcomingActivity = await GetUserActivity(targetUser);
                await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, upcomingActivity);

                // Get private events and etchings
                nest.Events = (await targetUser.PastEvents).ConvertAll(e => e.ToEventThinSlice());
                nest.Events.AddRange(upcomingActivity.ConvertAll(e => new Event(e).ToEventThinSlice()));

                foreach (var thinSlice in nest.Events)
                {
                    Event @event = new(thinSlice);
                    nest.Etchings.AddRange(await @event.Etchings);
                }
            }
            // Check if users are friends
            else if (await targetUser.IsFriendsWith(user))
            {
                // Gather active and upcoming events visible to the user
                var upcomingActivity = await GetUserActivity(targetUser);
                await Terminal.EventDirector.RemoveInaccessibleEventsAsync(user, upcomingActivity);

                // Get private events and etchings
                nest.Events = (await targetUser.PastEvents).ConvertAll(e => e.ToEventThinSlice());
                nest.Events.AddRange(upcomingActivity.ConvertAll(e => new Event(e).ToEventThinSlice()));

                nest.Etchings = await Etchings.GetEtchingsByUserAsync(targetUser.Id);
            }
            else
            {
                // Get public hosted events
                var hostedEvents = (await Events.FindEventsByUserAsync(targetUser.Id)).ConvertAll(e => new Event(e));
                nest.Events = hostedEvents.ConvertAll(e => e.ToEventThinSlice());

                // Get common events
                var commonEvents = (await targetUser.PastEvents)
                    .Except(hostedEvents)
                    .Intersect(await user.PastEvents)
                    .ToList().ConvertAll(e => e.ToEventThinSlice());

                nest.Events.AddRange(commonEvents);

                var targetEtchings = await Etchings.GetEtchingsByUserAsync(targetUser.Id);

                nest.Etchings.AddRange(targetEtchings.Where(etching => commonEvents.Exists(e => e.Id.Equals(etching.EventId))));
            }

            return nest;
        }

        public async Task<List<EventShard>> GetUserActivityAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Verify users are friends
            Try(user.Equals(targetUser) || await targetUser.IsFriendsWith(user),
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

            ConcurrentDictionary<UserSilhouette, List<EventShard>> friendEvents = new();

            // Gather visible activity of each friend
            (await user.Friends).AsParallel()
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
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            Fail(user.Equals(targetUser),
                new InvalidUserException("User cannot follow themself."));

            Fail(await user.IsBlocking(targetUser) || await user.IsBlockedBy(targetUser),
                new InvalidUserException("User cannot follow blocked/blocking user."));

            await Profiles.FollowUserAsync(userId, targetId, Psijic.Time);
        }

        public async Task UnfollowUserAsync(ulong userId, ulong targetId)
        {
            await Profiles.UnfollowUserAsync(userId, targetId);
        }

        public async Task BlockUserAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

			Fail(user.Equals(targetUser),
				new InvalidUserException("User cannot block themself."));

			await Profiles.BlockUserAsync(userId, targetId, Psijic.Time);
        }

        public async Task UnblockUserAsync(ulong userId, ulong targetId)
        {
            await Profiles.UnblockUserAsync(userId, targetId);
        }

        public async Task RateUserAsync(ulong userId, ulong targetId, UserRating rating)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            Fail(user.Equals(targetUser),
                new InvalidUserException("User cannot rate themself."));

            // Check if rating is to remove
            if (rating != UserRating.Remove)
            {
                await Profiles.RateUserAsync(userId, targetId, rating, Psijic.Time);
            }
            else
            {
                await Profiles.RemoveUserRatingAsync(userId, targetId);
            }

            await targetUser.CalculateReputation();
            _ = Accounts.UpdateUserAsync(targetId, new() { (nameof(UserShard.Reputation), targetUser.Reputation) });
        }

		#endregion

		#region Favours

        internal async Task<List<User>> RequestFriendsAsync(User user)
        {
            return (await Profiles.GetFriendsAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

        internal async Task<List<User>> RequestFollowedUsersAsync(User user)
        {
            return (await Profiles.GetFollowedUsersAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

        internal async Task<List<User>> RequestFollowersAsync(User user)
        {
            return (await Profiles.GetUsersFollowingAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

		internal async Task<List<User>> RequestBlockedUsersAsync(User user)
        {
            return (await Profiles.GetBlockedUsersAsync(user.Id))
				.ConvertAll(user => new User(user));
        }

		internal async Task<List<User>> RequestUsersBlockingAsync(User user)
        {
            return (await Profiles.GetUsersBlockingAsync(user.Id))
				.ConvertAll(user => new User(user));
        }

        internal async Task<(int Positive, int Negative)> RequestAllRatingsAsync(User user)
            => await Profiles.GetUserRatingsAsync(user.Id);

		#endregion

		#region Tools

		private async Task<List<EventShard>> GetUserActivity(User user)
        {
            _ = user.UpcomingEvents.Sync();
            _ = user.CurrentEvent.Sync();
            
            // Gather all user event data
            var upcomingActivity = await user.UpcomingEvents;

            if (!(await user.CurrentEvent).Equals(Event.None))
            { upcomingActivity.Add(await user.CurrentEvent); }

            return upcomingActivity
				.ConvertAll(@event => @event.ToEventShard());
        }

		#endregion
	}
}

