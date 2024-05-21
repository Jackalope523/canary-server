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
    internal class NestDirector : AbstractDirector, INestOperations
	{
		#region Initialisation

		public NestDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

        public async Task<NestShard> GetUserNestAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Fail if user is blocked
            Fail(await user.IsBlockedBy(targetUser),
                new InvalidUserException("User is unable to view target."));

            NestShard nest = new(user.ToUserProfile(), new(), new());

            // Check if user is themself
            if (user.Equals(targetUser))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingAgenda = await GetUserAgenda(targetUser);
                upcomingAgenda = await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, upcomingAgenda);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    Gatherings = (await targetUser.PastGatherings).ConvertAll(e => e.ToGatheringShard())
                };

                nest.Gatherings.AddRange(upcomingAgenda.Agenda.ConvertAll(e => new Gathering(e.Gathering).ToGatheringShard()));

                foreach (var shard in nest.Gatherings)
                {
                    Gathering @gathering = new(shard);
                    nest.Snapshots.AddRange(await @gathering.Snapshots);
                }
            }
            // Check if users are companions
            else if (await targetUser.IsCompanionsWith(user))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingAgenda = await GetUserAgenda(targetUser);
                upcomingAgenda = await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, upcomingAgenda);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    Gatherings = (await targetUser.PastGatherings).ConvertAll(e => e.ToGatheringShard())
                };

                nest.Gatherings.AddRange(upcomingAgenda.Agenda.ConvertAll(e => new Gathering(e.Gathering).ToGatheringShard()));

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

        public async Task<AgendaShard> GetUserAgendaAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Verify users are companions
            Try(user.Equals(targetUser) || await targetUser.IsCompanionsWith(user),
                new InvalidUserException("User is unable to view target."));

            // Gather active and upcoming gatherings
            var upcomingAgenda = await GetUserAgenda(targetUser);

            // Remove active and upcoming gatherings if the user cannot view them
            await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, upcomingAgenda);

            return upcomingAgenda;
        }

        public async Task<IDictionary<UserSilhouette, AgendaShard>> GetCompanionAgendaAsync(ulong userId)
        {
            var user = await GetUserAsync(userId);

            ConcurrentDictionary<UserSilhouette, AgendaShard> companionGatherings = new();

            // Gather visible agenda of each companion
            (await user.Companions).AsParallel()
                .ForAll(async companion =>
                {
                    var companionAgenda = await GetUserAgenda(companion);
                    await Terminal.GatheringDirector.RemoveInaccessibleGatheringBondsAsync(user, companionAgenda);
                    companionGatherings.TryAdd(companion.ToUserSilhouette(), companionAgenda);
                });

            return companionGatherings;
        }

        public async Task<List<UserSilhouette>> GetCompanionsAsync(ulong userId)
        {
            return await Nests.GetCompanionsAsync(userId);
        }

        public async Task<List<UserSilhouette>> GetAppreciatedUsersAsync(ulong userId)
        {
            return await Nests.GetAppreciatedUsersAsync(userId);
        }

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(ulong userId)
        {
            return await Nests.GetBlockedUsersAsync(userId);
        }

        public async Task AppreciateUserAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            Fail(user.Equals(targetUser),
                new InvalidUserException("User cannot appreciate themself."));

            Fail(await user.IsBlocking(targetUser) || await user.IsBlockedBy(targetUser),
                new InvalidUserException("User cannot appreciate blocked/blocking user."));

            await Nests.AppreciateUserAsync(userId, targetId, Psijic.Time);
        }

        public async Task UnappreciateUserAsync(ulong userId, ulong targetId)
        {
            await Nests.UnappreciateUserAsync(userId, targetId);
        }

        public async Task BlockUserAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

			Fail(user.Equals(targetUser),
				new InvalidUserException("User cannot block themself."));

			await Nests.BlockUserAsync(userId, targetId, Psijic.Time);
        }

        public async Task UnblockUserAsync(ulong userId, ulong targetId)
        {
            await Nests.UnblockUserAsync(userId, targetId);
        }

		#endregion

		#region Favours

        internal async Task<List<User>> RequestCompanionsAsync(User user)
        {
            return (await Nests.GetCompanionsAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

        internal async Task<List<User>> RequestAppreciatedUsersAsync(User user)
        {
            return (await Nests.GetAppreciatedUsersAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

        internal async Task<List<User>> RequestAppreciateersAsync(User user)
        {
            return (await Nests.GetUsersAppreciatingAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

		internal async Task<List<User>> RequestBlockedUsersAsync(User user)
        {
            return (await Nests.GetBlockedUsersAsync(user.Id))
				.ConvertAll(user => new User(user));
        }

		internal async Task<List<User>> RequestUsersBlockingAsync(User user)
        {
            return (await Nests.GetUsersBlockingAsync(user.Id))
				.ConvertAll(user => new User(user));
        }

        internal async Task<(int Positive, int Negative)> RequestAllRatingsAsync(User user)
            => await Nests.GetUserRatingsAsync(user.Id);

		#endregion

		#region Tools

        private async Task<AgendaShard> GetUserAgenda(User user)
        {
            _ = user.UpcomingGatherings.Sync();
            _ = user.CurrentGathering.Sync();

            // Gather all user gathering data
            AgendaShard agenda = new((await user.UpcomingGatherings)
                .ConvertAll(@gathering => (@gathering.ToGatheringShard(), GatheringBond.Guest)));

            agenda.Agenda.AddRange((await user.SurveyingGatherings)
                .ConvertAll(@gathering => (@gathering.ToGatheringShard(), GatheringBond.Surveying)));

            if (!(await user.CurrentGathering).Equals(Gathering.None))
            { agenda.Agenda.Add(((await user.CurrentGathering).ToGatheringShard(), GatheringBond.Arrived)); }

            return agenda;
        }

        #endregion
    }
}

