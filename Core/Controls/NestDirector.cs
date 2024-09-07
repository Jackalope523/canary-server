using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Microsoft.Extensions.Logging;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class NestDirector : AbstractDirector, INestOperations
	{
		#region Initialisation

		public NestDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

        public async Task<NestShard> GetNestAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Fail if user is blocked
            FailIf(await user.IsBlockedBy(targetUser),
                new InvalidUserException("User is unable to view target."));

            NestShard nest = new(new());

            // Check if user is themself
            if (user.Equals(targetUser))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingAgendaSync = RequestAgenda(user);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    GatheringIds = (await targetUser.PastGatherings).ConvertAll(e => e.Id)
                };

                nest.GatheringIds.AddRange((await upcomingAgendaSync).Agenda
                    .Where(bond => !bond.Bond.Equals(GatheringBond.Watching)).ToList()
                    .ConvertAll(e => new Gathering(e.Gathering).Id));
            }
            // Check if users are companions
            else if (await targetUser.IsCompanionsWith(user))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingAgendaSync = RequestAgenda(targetUser);
                var siftedAgendaSync = Terminal.GatheringDirector.RemoveUnviewableGatheringBondsAsync(user, await upcomingAgendaSync);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    GatheringIds = (await targetUser.PastGatherings).ConvertAll(e => e.Id)
                };

                nest.GatheringIds.AddRange((await siftedAgendaSync).Agenda
                    .Where(bond => !bond.Bond.Equals(GatheringBond.Watching)).ToList()
                    .ConvertAll(e => new Gathering(e.Gathering).Id));
            }
            // User is a stranger
            else
            {
                // Get public hosted gatherings
                var hostedGatherings = (await Gatherings.FindGatheringsByUserAsync(targetUser.Id)).ConvertAll(e => new Gathering(e));
                nest = nest with
                {
                    GatheringIds = hostedGatherings.ConvertAll(e => e.Id)
                };

                // Get common gatherings
                var commonGatherings = (await targetUser.PastGatherings)
                    .Except(hostedGatherings)
                    .Intersect(await user.PastGatherings)
                    .ToList().ConvertAll(e => e.Id);

                nest.GatheringIds.AddRange(commonGatherings);
            }

            return nest;
        }

        public async Task<AgendaShard> GetUserAgendaAsync(ulong userId)
        {
            var user = await GetUserAsync(userId);

            // Gather active and upcoming gatherings
            var upcomingAgenda = await RequestAgenda(user);

            return upcomingAgenda;
        }

        public async Task<IDictionary<ulong, AgendaShard>> GetCompanionAgendasAsync(ulong userId)
        {
            var user = await GetUserAsync(userId);

            Dictionary<ulong, AgendaShard> companionGatherings = new();

            // Gather visible agenda of each companion
            foreach (var companion in await user.Companions)
            {
                var companionAgenda = await RequestAgenda(companion);
                companionAgenda = await Terminal.GatheringDirector.RemoveUnviewableGatheringBondsAsync(user, companionAgenda);
                companionGatherings.TryAdd(companion.Id, companionAgenda);
            };

            return companionGatherings;
        }

        public async Task<List<UserShard>> GetCompanionsAsync(ulong userId)
        {
            return await Nests.GetCompanionsAsync(userId);
        }

        public async Task<List<UserShard>> GetAppreciatedUsersAsync(ulong userId)
        {
            return await Nests.GetAppreciatedUsersAsync(userId);
        }

        public async Task<List<UserShard>> GetBlockedUsersAsync(ulong userId)
        {
            return await Nests.GetBlockedUsersAsync(userId);
        }

        public async Task AppreciateUserAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            FailIf(user.Equals(targetUser),
                new InvalidUserException("User cannot appreciate themself."));

            FailIf(await user.IsBlocking(targetUser) || await user.IsBlockedBy(targetUser),
                new InvalidUserException("User cannot appreciate blocked/blocking user."));

            await Nests.AppreciateUserAsync(userId, targetId, Psijic.Time);

            _ = targetUser.PostTelegram(user, TelegramMessage.UserAppreciated, "");
        }

        public async Task UnappreciateUserAsync(ulong userId, ulong targetId)
        {
            await Nests.UnappreciateUserAsync(userId, targetId);
        }

        public async Task BlockUserAsync(ulong userId, ulong targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

			FailIf(user.Equals(targetUser),
				new InvalidUserException("User cannot block themself."));

			await Nests.BlockUserAsync(userId, targetId, Psijic.Time);

            // Ensure removal from upcoming hosted gatherings
            foreach (var gathering in await user.UpcomingGatherings)
            {
                // Check if user is host
                if (gathering.Host.Equals(user))
                {
                    try
                    {
                        // Blind-remove target
                        await Gatherings.DeleteUserStateAsync(targetUser.Id, gathering.Id);
                    }
                    catch { }
                }
            }
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

		#endregion

		#region Tools

        private async Task<AgendaShard> RequestAgenda(User user)
        {
            _ = user.UpcomingGatherings.Sync();
            _ = user.CurrentGathering.Sync();

            // Gather all user gathering data
            AgendaShard agenda = new((await user.UpcomingGatherings)
                .ConvertAll(gathering => new AgendaBondPair(gathering.ToGatheringShard(), GatheringBond.Guest)));

            agenda.Agenda.AddRange((await user.SurveyingGatherings)
                .ConvertAll(gathering => new AgendaBondPair(gathering.ToGatheringShard(), GatheringBond.Watching)));

            if (!(await user.CurrentGathering).Equals(Gathering.None))
            { agenda.Agenda.Add(new AgendaBondPair((await user.CurrentGathering).ToGatheringShard(), GatheringBond.Arrived)); }

            return agenda;
        }

        #endregion
    }
}

