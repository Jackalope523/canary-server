using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Core.Notifications;
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

        public async Task<NestShard> GetNestAsync(long userId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            // Fail if user is blocked
            FailIf(await user.IsBlockedBy(targetUser),
                new UserErrorException(UserErrorCode.CANNOT_VIEW));

            NestShard nest = new(new());

            // Check if user is themself
            if (user.Equals(targetUser))
            {
                // Gather active and upcoming gatherings visible to the user
                var upcomingAgendaSync = RequestAgenda(user);

                // Get private gatherings and snapshots
                nest = nest with
                {
                    Twigs = (await targetUser.PastGatherings).ConvertAll(e => e.ToTwigShard())
                };

                nest.Twigs.AddRange((await upcomingAgendaSync).Cards
                    .ConvertAll(card => new TwigShard(card.GatheringId, card.StartTime)));
            }
            // Check if users are companions
            else if (await targetUser.IsCompanionsWith(user))
            {
                var hasMutualSync = Nests.HaveMutualGathering(user.Id, targetUser.Id);

                // Gather active and upcoming gatherings visible to the user
                var upcomingAgendaSync = RequestAgenda(targetUser);
                var siftedAgendaSync = Terminal.GatheringDirector.RemoveUnviewableAgendaCardsAsync(user, await upcomingAgendaSync);

                // Get private gatherings and snapshots
                var twigs = (await targetUser.PastGatherings).ConvertAll(e => e.ToTwigShard());

                twigs.AddRange((await siftedAgendaSync).Cards
                    .ConvertAll(card => new TwigShard(card.GatheringId, card.StartTime)));

                if (await hasMutualSync)
                {
                    nest = new(twigs, (await Nests.GetFirstMutualGathering(user.Id, targetUser.Id)).Id);
                }
                else
                {
                    nest = new(twigs, default);
                }
            }
            // User is a stranger
            else
            {
                // Check if has a mutual gathering
                var hasMutualSync = Nests.HaveMutualGathering(user.Id, targetUser.Id);

                // Get public hosted gatherings
                var hostedGatherings = (await Gatherings.FindGatheringsByUserAsync(targetUser.Id)).ConvertAll(e => new Gathering(e));
                var twigs = hostedGatherings.ConvertAll(e => e.ToTwigShard());

                // Get common gatherings
                var commonGatherings = (await targetUser.PastGatherings)
                    .Except(hostedGatherings)
                    .Intersect(await user.PastGatherings)
                    .ToList().ConvertAll(e => e.ToTwigShard());

                twigs.AddRange(commonGatherings);

                if (await hasMutualSync)
                {
                    nest = new(twigs, (await Nests.GetLatestMutualGathering(user.Id, targetUser.Id)).Id);
                }
                else
                {
                    nest = new(twigs, default);
                }
            }

            return nest;
        }

        public async Task<AgendaShard> GetUserAgendaAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            // Gather active and upcoming gatherings
            var upcomingAgenda = await RequestAgenda(user);

            return upcomingAgenda;
        }

        public async Task<IDictionary<long, AgendaShard>> GetCompanionAgendasAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            Dictionary<long, AgendaShard> companionGatherings = new();

            // Gather visible agenda of each companion
            foreach (var companion in await user.Companions)
            {
                var companionAgenda = await RequestAgenda(companion);
                companionAgenda = await Terminal.GatheringDirector.RemoveUnviewableAgendaCardsAsync(user, companionAgenda);
                companionGatherings.TryAdd(companion.Id, companionAgenda);
            };

            return companionGatherings;
        }

        public async Task<List<UserShard>> GetCompanionsAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            return (await user.Companions)
                .ConvertAll(u => u.ToUserShard());
        }

        public async Task<List<UserShard>> GetIncomingCompanionshipRequestsAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            var userFollowers = await user.Followers;
            var userCompanions = await user.Companions;

            // Strip out companions
            return userFollowers.Except(userCompanions)
                .ToList()
                .ConvertAll(u => u.ToUserShard());
        }

        public async Task<List<UserShard>> GetOutgoingCompanionshipRequestsAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            var userFollowing = await user.Following;
            var userCompanions = await user.Companions;

            // Strip out companions
            return userFollowing.Except(userCompanions)
                .ToList()
                .ConvertAll(u => u.ToUserShard());
        }

        public async Task<List<UserShard>> GetRecentlyMetAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            // Get last gathering
            var lastGathering = await user.LastGathering();

            var neutralGuests = await Task.WhenAll((await lastGathering.Left)
                .Select(async guest =>
                    {
                        if (await user.IsNeutralOrUnrequitedWith(guest))
                        {
                            return guest;
                        }

                        return User.Hidden;
                    }
                ));

            // Remove all Hidden users and self
            return neutralGuests
                .Where(guest => !guest.Equals(User.Hidden) && !guest.Equals(user))
                .ToList()
                .ConvertAll(u => u.ToUserShard());
        }

        public async Task<List<BlockedUserShard>> GetBlockedUsersAsync(long userId)
        {
            return await Nests.GetBlockedUsersAsync(userId);
        }

        public async Task AcceptOrRequestCompanionshipAsync(long userId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            FailIf(user.Equals(targetUser),
                new UserErrorException(UserErrorCode.CANNOT_FOLLOW_SELF));

            Verify(await user.CanFollow(targetUser),
                new UserErrorException(UserErrorCode.CANNOT_FOLLOW));

            await Nests.FollowUserAsync(user.Id, targetUser.Id, Psijic.Time);

            CanaryNotification targetNotification = CanaryNotification.CompanionshipRequest(user.ToUserShard());

            // Should always hit
            if (await Nests.HaveMutualGathering(user.Id, targetUser.Id))
            {
                var lastMutualGathering = await Nests.GetLatestMutualGathering(user.Id, targetUser.Id);

                targetNotification = CanaryNotification.CompanionshipRequest(user.ToUserShard(), lastMutualGathering.Title);
            }

            // Check if this forges companionship
            if (await targetUser.IsFollowing(user))
            {
                targetNotification = CanaryNotification.CompanionshipForged(user.ToUserShard());
            }
            else
            {
                _ = targetUser.PostTelegram(user, TelegramMessage.UserFollowed, "");
            }

            _ = targetUser.Notify(targetNotification);
        }

        public async Task RequestCompanionshipAsync(long userId, string code)
        {
            var user = await GetUserAsync(userId);

            CoreUser potentialUser;

            // Check user code
            try
            {
                potentialUser = await Accounts.FindUserByCodeAsync(code.ToLower());
            }
            catch
            { throw new UserErrorException(UserErrorCode.CODE_NOT_FOUND); }

            User targetUser = new(potentialUser);

            FailIf(user.Equals(targetUser),
                new UserErrorException(UserErrorCode.CANNOT_FOLLOW_SELF));

            Verify(await user.CanFollow(targetUser, hasCode: true),
                new UserErrorException(UserErrorCode.CANNOT_FOLLOW));

            await Nests.FollowUserAsync(user.Id, targetUser.Id, Psijic.Time);

            CanaryNotification targetNotification = CanaryNotification.CompanionshipRequest(user.ToUserShard());

            // Check if this forges companionship
            if (await targetUser.IsFollowing(user))
            {
                targetNotification = CanaryNotification.CompanionshipForged(user.ToUserShard());
            }
            else
            {
                _ = targetUser.PostTelegram(user, TelegramMessage.UserFollowed, "");
            }

            _ = targetUser.Notify(targetNotification);
        }

        public async Task DenyOrRemoveUserAsync(long userId, long targetId)
        {
            await Nests.UnfollowUserAsync(targetId, userId);
            await Nests.UnfollowUserAsync(userId, targetId);
        }

        public async Task BlockUserAsync(long userId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

			FailIf(user.Equals(targetUser),
				new UserErrorException(UserErrorCode.CANNOT_BLOCK_SELF));

			await Nests.BlockUserAsync(userId, targetId, Psijic.Time);

            // Ensure removal from upcoming hosted gatherings
            foreach (var gathering in await user.UpcomingGatherings)
            {
                // Check if user is host
                if (gathering.HostId.Equals(user.Id))
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

        public async Task UnblockUserAsync(long userId, long targetId)
        {
            await Nests.UnblockUserAsync(userId, targetId);
        }

        public async Task<bool> AuthorisedToFollow(long userId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);

            return await user.CanFollow(targetUser);
        }

		#endregion

		#region Favours

        internal async Task<List<User>> RequestCompanionsAsync(User user)
        {
            return (await Nests.GetCompanionsAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

        internal async Task<List<User>> RequestFollowedUsersAsync(User user)
        {
            return (await Nests.GetFollowedUsersAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

        internal async Task<List<User>> RequestFollowersAsync(User user)
        {
            return (await Nests.GetUserFollowersAsync(user.Id))
                .ConvertAll(user => new User(user));
		}

		internal async Task<List<User>> RequestBlockedUsersAsync(User user)
        {
            return (await Nests.GetBlockedUsersAsync(user.Id))
				.ConvertAll(u => User.GetUserAsync(u.Id).Result);
        }

		internal async Task<List<User>> RequestUsersBlockingAsync(User user)
        {
            return (await Nests.GetUsersBlockingAsync(user.Id))
				.ConvertAll(user => new User(user));
        }

        internal async Task<bool> RequestAttendedMutualGatheringAsync(User user, User target)
        {
            return await Nests.HaveMutualGathering(user.Id, target.Id);
        }

		#endregion

		#region Tools

        private async Task<AgendaShard> RequestAgenda(User user)
        {
            _ = user.UpcomingGatherings.Sync();
            _ = user.CurrentGathering.Sync();

            // Gather all user gathering data
            AgendaShard agenda = new((await user.UpcomingGatherings)
                .ConvertAll(gathering => new CardShard(gathering.Id, gathering.StartTime, GatheringBond.Guest)));

            if (!(await user.CurrentGathering).Equals(Gathering.None))
            { agenda.Cards.Add(new CardShard((await user.CurrentGathering).Id, (await user.CurrentGathering).StartTime, GatheringBond.Arrived)); }

            return agenda;
        }

        #endregion
    }
}

