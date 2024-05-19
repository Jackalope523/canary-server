using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

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

        public async Task<NestShard> GetUserNestAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Fail if user is blocked
            Fail(await user.IsBlockedBy(targetUser),
                new InvalidUserException("User is unable to view target."));

            NestShard nest = new(new(), new());

            // Check if user is themself
            if (user.Equals(targetUser))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingActivity = await GetUserActivity(targetUser);
                upcomingActivity = await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, upcomingActivity);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    Gatherings = (await targetUser.PastGatherings).ConvertAll(e => e.ToGatheringShard())
                };

                nest.Gatherings.AddRange(upcomingActivity.Activity.ConvertAll(e => new Gathering(e.Gathering).ToGatheringShard()));

                foreach (var shard in nest.Gatherings)
                {
                    Gathering @gathering = new(shard);
                    nest.Snapshots.AddRange(await @gathering.Snapshots);
                }
            }
            // Check if users are friends
            else if (await targetUser.IsFriendsWith(user))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingActivity = await GetUserActivity(targetUser);
                upcomingActivity = await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, upcomingActivity);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    Gatherings = (await targetUser.PastGatherings).ConvertAll(e => e.ToGatheringShard())
                };

                nest.Gatherings.AddRange(upcomingActivity.Activity.ConvertAll(e => new Gathering(e.Gathering).ToGatheringShard()));

                nest = nest with
                {
                    Snapshots = await Snapshots.GetSnapshotsByUserAsync(targetUser.Id)
                };
            }
            else
            {
                // Get public hosted gatherings
                var hostedGatherings = (await Gatherings.FindGatheringsByUserAsync(targetUser.Id)).ConvertAll(e => new Gathering(e));
                nest = nest with
                {
                    Gatherings = hostedGatherings.ConvertAll(e => e.ToGatheringShard())
                };

                // Get common gatherings
                var commonGatherings = (await targetUser.PastGatherings)
                    .Except(hostedGatherings)
                    .Intersect(await user.PastGatherings)
                    .ToList().ConvertAll(e => e.ToGatheringShard());

                nest.Gatherings.AddRange(commonGatherings);

                var targetSnapshots = await Snapshots.GetSnapshotsByUserAsync(targetUser.Id);

                nest.Snapshots.AddRange(targetSnapshots.Where(snapshot => commonGatherings.Exists(e => e.Id.Equals(snapshot.GatheringId))));
            }

            return nest;
        }

        public async Task<ActivityShard> GetUserActivityAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Verify users are friends
            Try(user.Equals(targetUser) || await targetUser.IsFriendsWith(user),
                new InvalidUserException("User is unable to view target."));

            // Gather active and upcoming gatherings
            var upcomingActivity = await GetUserActivity(targetUser);

            // Remove active and upcoming gatherings if the user cannot view them
            await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, upcomingActivity);

            return upcomingActivity;
        }

        public async Task<IDictionary<UserSilhouette, ActivityShard>> GetFriendActivityAsync(ulong userId)
        {
            var user = await GetUserAsync(userId);

            ConcurrentDictionary<UserSilhouette, ActivityShard> friendGatherings = new();

            // Gather visible activity of each friend
            (await user.Friends).AsParallel()
                .ForAll(async friend =>
                {
                    var friendActivity = await GetUserActivity(friend);
                    await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, friendActivity);
                    friendGatherings.TryAdd(friend.ToUserSilhouette(), friendActivity);
                });

            return friendGatherings;
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
            _ = Accounts.UpdateUserAsync(targetId, new() { (nameof(CoreUser.Reputation), targetUser.Reputation) });
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

        private async Task<ActivityShard> GetUserActivity(User user)
        {
            _ = user.UpcomingGatherings.Sync();
            _ = user.CurrentGathering.Sync();

            // Gather all user gathering data
            ActivityShard activity = new((await user.UpcomingGatherings)
                .ConvertAll(@gathering => (@gathering.ToGatheringShard(), GatheringBond.Guest)));

            activity.Activity.AddRange((await user.SurveyingGatherings)
                .ConvertAll(@gathering => (@gathering.ToGatheringShard(), GatheringBond.Surveying)));

            if (!(await user.CurrentGathering).Equals(Gathering.None))
            { activity.Activity.Add(((await user.CurrentGathering).ToGatheringShard(), GatheringBond.Arrived)); }

            return activity;
        }

        #endregion
    }
}

