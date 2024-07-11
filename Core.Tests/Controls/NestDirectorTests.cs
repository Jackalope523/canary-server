using Core.Boundaries;
using Core.Controls;
using Core.Entities;

using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Controls
{
    public class NestDirectorTests : CoreTest
    {
		private NestDirector director;

        public NestDirectorTests()
        {
			director = environment.Terminal.NestDirector;
        }

		[Fact]
		public async Task GetUserNestAsync_Self_ReturnsNest()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();

			var hostedGathering = await environment.GeneratePastGatheringAsync(user);
			var attendedGathering = await environment.GeneratePastGatheringAsync(host, user);
			var unattendedGathering = await environment.GeneratePastGatheringAsync(host);
			var ongoingGathering = await environment.GenerateUpcomingGatheringAsync(host, user);

			var funLovingSnapshot = await environment.GenerateSnapshotAsync(attendedGathering, user);
			var lessLovingSnapshot = await environment.GenerateSnapshotAsync(attendedGathering, host);
			var okSnapshot = await environment.GenerateSnapshotAsync(ongoingGathering, user);

			// Act
			var nest = await director.GetUserNestAsync(user.Id, user.Id);

			// Assert
			Assert.Equal(3, nest.Gatherings.Count);
			Assert.Equal(hostedGathering.ToGatheringShard(), nest.Gatherings.Find(e => e.Id.Equals(hostedGathering.Id)));
			Assert.Equal(attendedGathering.ToGatheringShard(), nest.Gatherings.Find(e => e.Id.Equals(attendedGathering.Id)));
			Assert.Equal(ongoingGathering.ToGatheringShard(), nest.Gatherings.Find(e => e.Id.Equals(ongoingGathering.Id)));

			Assert.Equal(3, nest.Snapshots.Count);
			Assert.Equal(funLovingSnapshot, nest.Snapshots.Find(e => e.Id.Equals(funLovingSnapshot.Id)));
			Assert.Equal(lessLovingSnapshot, nest.Snapshots.Find(e => e.Id.Equals(lessLovingSnapshot.Id)));
			Assert.Equal(okSnapshot, nest.Snapshots.Find(e => e.Id.Equals(okSnapshot.Id)));
		}

		[Fact]
		public async Task GetUserNestAsync_Companion_ReturnsNest()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var companion = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(user, companion);

			var hostedGathering = await environment.GeneratePastGatheringAsync(user, companion);
			var mutuallyAttendedGathering = await environment.GeneratePastGatheringAsync(host, user, companion);
			var unattendedGathering = await environment.GeneratePastGatheringAsync(companion);
			var ongoingGathering = await environment.GenerateUpcomingGatheringAsync(host, companion);

			var userSnapshot = await environment.GenerateSnapshotAsync(hostedGathering, user);
			var companionSnapshot = await environment.GenerateSnapshotAsync(mutuallyAttendedGathering, companion);
			var hostSnapshot = await environment.GenerateSnapshotAsync(mutuallyAttendedGathering, host);
			var ongoingGatheringCompanionSnapshot = await environment.GenerateSnapshotAsync(ongoingGathering, companion);
			var unattendedGatheringCompanionSnapshot = await environment.GenerateSnapshotAsync(unattendedGathering, companion);

			// Act
			var nest = await director.GetUserNestAsync(user.Id, companion.Id);

            // Assert
            Assert.Equal(4, nest.Gatherings.Count);

			Assert.Equal(3, nest.Snapshots.Count);
		}

		[Fact]
		public async Task GetUserNestAsync_NeutralHost_ReturnsPublicNest()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			var mutuallyAttendedGathering = await environment.GeneratePastGatheringAsync(user, randomUser);
			var unattendedGathering = await environment.GeneratePastGatheringAsync(randomUser);

			var mutualGatheringSnapshot = await environment.GenerateSnapshotAsync(mutuallyAttendedGathering, randomUser);
			var unattendedGatheringSnapshot = await environment.GenerateSnapshotAsync(unattendedGathering, randomUser);

			// Act
			var nest = await director.GetUserNestAsync(user.Id, randomUser.Id);

            // Assert
            Assert.Equal(2, nest.Gatherings.Count);

			Assert.Single(nest.Snapshots);
		}

		[Fact]
		public async Task GetUserNestAsync_Blocked_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var nest = director.GetUserNestAsync(user.Id, enemy.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await nest);
		}

		[Fact]
		public async Task GetUserAgendaAsync_Self_ReturnsAgenda()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();

			var pastGathering = await environment.GeneratePastGatheringAsync(user);
			var ongoingGathering = await environment.GenerateOngoingGatheringAsync(user);
			var upcomingGathering = await environment.GenerateUpcomingGatheringAsync(host, user);
			var anotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(user);

			// Act
			var agenda = await director.GetUserAgendaAsync(user.Id, user.Id);

			// Assert
			Assert.Equal(3, agenda.Agenda.Count);
		}

		[Fact]
		public async Task GetUserAgendaAsync_Companion_ReturnsAgenda()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var companion = await environment.GenerateUniqueUserAsync();
			var randomHost = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(user, companion);

			var pastGathering = await environment.GeneratePastGatheringAsync(companion);
			var ongoingGathering = await environment.GenerateOngoingGatheringAsync(companion);
			var upcomingGathering = await environment.GenerateUpcomingGatheringAsync(user, companion);
			var anotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(randomHost, companion);
			var yetAnotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(companion);

			// Act
			var agenda = await director.GetUserAgendaAsync(user.Id, companion.Id);

			// Assert
			Assert.Equal(4, agenda.Agenda.Count);
		}

		[Fact]
		public async Task GetUserAgendaAsync_Neutral_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var agenda = director.GetUserAgendaAsync(user.Id, randomUser.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await agenda);
		}

		[Fact]
		public async Task GetUserAgendaAsync_Blocked_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var agenda = director.GetUserAgendaAsync(user.Id, enemy.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await agenda);
		}

		[Fact]
		public async Task GetCompanionAgendaAsync_ReturnsCompanionAgenda()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var activeCompanion = await environment.GenerateUniqueUserAsync();
			var sloadButChill = await environment.GenerateUniqueUserAsync();
			var randomHost = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(user, activeCompanion, sloadButChill);

			var pastGathering = await environment.GeneratePastGatheringAsync(activeCompanion);
			var ongoingGathering = await environment.GenerateOngoingGatheringAsync(activeCompanion, sloadButChill);
			var upcomingGathering = await environment.GenerateUpcomingGatheringAsync(user, activeCompanion);
			var anotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(randomHost, activeCompanion);
			var yetAnotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(activeCompanion);

			// Act
			var agenda = await director.GetCompanionAgendaAsync(user.Id);

			// Assert
			Assert.Equal(2, agenda.Keys.Count);
			Assert.Equal(4, agenda[activeCompanion.ToUserShard()].Agenda.Count);
			Assert.Single(agenda[sloadButChill.ToUserShard()].Agenda);
		}

		[Fact]
		public async Task GetCompanionsAsync_ReturnsCompanions()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var companion = await environment.GenerateUniqueUserAsync();
			var otherCompanion = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(user, companion, otherCompanion);

			// Act
			var companions = await director.GetCompanionsAsync(user.Id);

			// Assert
			Assert.Equal(2, companions.Count);

		}

		[Fact]
		public async Task GetAppreciatedUsersAsync_ReturnsUsers()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var companion = await environment.GenerateUniqueUserAsync();
			var otherCompanion = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(user, companion, otherCompanion);

			// Act
			var appreciatedUsers = await director.GetAppreciatedUsersAsync(user.Id);

			// Assert
			Assert.Equal(2, appreciatedUsers.Count);
		}

		[Fact]
		public async Task GetBlockedUsersAsync_ReturnsUsers()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var notACompanion = await environment.GenerateUniqueUserAsync();
			var archNemesis = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, notACompanion, archNemesis);

			// Act
			var blockedUsers = await director.GetBlockedUsersAsync(user.Id);

			// Assert
			Assert.Equal(2, blockedUsers.Count);
		}

		[Fact]
		public async Task AppreciateUserAsync_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var stranger = await environment.GenerateUniqueUserAsync();

			// Act
			await director.AppreciateUserAsync(user.Id, stranger.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task AppreciateUserAsync_Blocked_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var action = director.AppreciateUserAsync(user.Id, enemy.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await action);
		}

		[Fact]
		public async Task UnappreciateUserAsync_AppreciatedUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var weirdDynamics = await environment.GenerateUniqueUserAsync();
			await director.AppreciateUserAsync(user.Id, weirdDynamics.Id);

			// Act
			await director.UnappreciateUserAsync(user.Id, weirdDynamics.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task BlockUserAsync_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var strangerDanger = await environment.GenerateUniqueUserAsync();

			// Act
			await director.BlockUserAsync(user.Id, strangerDanger.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task UnblockUserAsync_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var weirdDynamics = await environment.GenerateUniqueUserAsync();
			await director.BlockUserAsync(user.Id, weirdDynamics.Id);

			// Act
			await director.UnblockUserAsync(user.Id, weirdDynamics.Id);
			// If no exception is thrown, the test is successful
		}
	}
}
