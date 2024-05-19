using Core.Boundaries;
using Core.Controls;
using Core.Entities;

using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Controls
{
    public class ProfileDirectorTests : CoreTest
    {
		private ProfileDirector director;

        public ProfileDirectorTests()
        {
			director = environment.Terminal.ProfileDirector;
        }

		[Fact]
		public async Task GetUserProfileAsync_Self_ReturnsProfile()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var profile = await director.GetUserProfileAsync(user.Id, user.Id);

			// Assert
			Assert.Equal(user.ToUserProfile(), profile);
		}

		[Fact]
		public async Task GetUserProfileAsync_Friend_ReturnsProfile()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);
			friend.NumberOfFollowers += 1;

			// Act
			var profile = await director.GetUserProfileAsync(user.Id, friend.Id);

			// Assert
			Assert.Equal(friend.ToUserProfile(), profile);
		}

		[Fact]
		public async Task GetUserProfileAsync_Neutral_ReturnsProfile()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var profile = await director.GetUserProfileAsync(user.Id, randomUser.Id);

			// Assert
			Assert.Equal(randomUser.ToUserProfile(), profile);
		}

		[Fact]
		public async Task GetUserProfileAsync_Blocked_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var profile = director.GetUserProfileAsync(user.Id, enemy.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await profile);
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
			var (Gatherings, Snapshots) = await director.GetUserNestAsync(user.Id, user.Id);

			// Assert
			Assert.Equal(3, Gatherings.Count);
			Assert.Equal(hostedGathering.ToGatheringShard(), Gatherings.Find(e => e.Id.Equals(hostedGathering.Id)));
			Assert.Equal(attendedGathering.ToGatheringShard(), Gatherings.Find(e => e.Id.Equals(attendedGathering.Id)));
			Assert.Equal(ongoingGathering.ToGatheringShard(), Gatherings.Find(e => e.Id.Equals(ongoingGathering.Id)));

			Assert.Equal(3, Snapshots.Count);
			Assert.Equal(funLovingSnapshot, Snapshots.Find(e => e.Id.Equals(funLovingSnapshot.Id)));
			Assert.Equal(lessLovingSnapshot, Snapshots.Find(e => e.Id.Equals(lessLovingSnapshot.Id)));
			Assert.Equal(okSnapshot, Snapshots.Find(e => e.Id.Equals(okSnapshot.Id)));
		}

		[Fact]
		public async Task GetUserNestAsync_Friend_ReturnsNest()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var hostedGathering = await environment.GeneratePastGatheringAsync(user, friend);
			var mutuallyAttendedGathering = await environment.GeneratePastGatheringAsync(host, user, friend);
			var unattendedGathering = await environment.GeneratePastGatheringAsync(friend);
			var ongoingGathering = await environment.GenerateUpcomingGatheringAsync(host, friend);

			var userSnapshot = await environment.GenerateSnapshotAsync(hostedGathering, user);
			var friendSnapshot = await environment.GenerateSnapshotAsync(mutuallyAttendedGathering, friend);
			var hostSnapshot = await environment.GenerateSnapshotAsync(mutuallyAttendedGathering, host);
			var ongoingGatheringFriendSnapshot = await environment.GenerateSnapshotAsync(ongoingGathering, friend);
			var unattendedGatheringFriendSnapshot = await environment.GenerateSnapshotAsync(unattendedGathering, friend);

			// Act
			var (Gatherings, Snapshots) = await director.GetUserNestAsync(user.Id, friend.Id);

			// Assert
			Assert.Equal(4, Gatherings.Count);

			Assert.Equal(3, Snapshots.Count);
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
			var (Gatherings, Snapshots) = await director.GetUserNestAsync(user.Id, randomUser.Id);

			// Assert
			Assert.Equal(2, Gatherings.Count);

			Assert.Single(Snapshots);
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
		public async Task GetUserAgendaAsync_Friend_ReturnsAgenda()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var randomHost = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var pastGathering = await environment.GeneratePastGatheringAsync(friend);
			var ongoingGathering = await environment.GenerateOngoingGatheringAsync(friend);
			var upcomingGathering = await environment.GenerateUpcomingGatheringAsync(user, friend);
			var anotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(randomHost, friend);
			var yetAnotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(friend);

			// Act
			var agenda = await director.GetUserAgendaAsync(user.Id, friend.Id);

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
		public async Task GetFriendAgendaAsync_ReturnsFriendAgenda()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var activeFriend = await environment.GenerateUniqueUserAsync();
			var sloadButChill = await environment.GenerateUniqueUserAsync();
			var randomHost = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, activeFriend, sloadButChill);

			var pastGathering = await environment.GeneratePastGatheringAsync(activeFriend);
			var ongoingGathering = await environment.GenerateOngoingGatheringAsync(activeFriend, sloadButChill);
			var upcomingGathering = await environment.GenerateUpcomingGatheringAsync(user, activeFriend);
			var anotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(randomHost, activeFriend);
			var yetAnotherUpcomingGathering = await environment.GenerateUpcomingGatheringAsync(activeFriend);

			// Act
			var agenda = await director.GetFriendAgendaAsync(user.Id);

			// Assert
			Assert.Equal(2, agenda.Keys.Count);
			Assert.Equal(4, agenda[activeFriend.ToUserSilhouette()].Agenda.Count);
			Assert.Single(agenda[sloadButChill.ToUserSilhouette()].Agenda);
		}

		[Fact]
		public async Task GetFriendsAsync_ReturnsFriends()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var otherFriend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend, otherFriend);

			// Act
			var friends = await director.GetFriendsAsync(user.Id);

			// Assert
			Assert.Equal(2, friends.Count);

		}

		[Fact]
		public async Task GetFollowedUsersAsync_ReturnsUsers()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var otherFriend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend, otherFriend);

			// Act
			var followedUsers = await director.GetFollowedUsersAsync(user.Id);

			// Assert
			Assert.Equal(2, followedUsers.Count);
		}

		[Fact]
		public async Task GetBlockedUsersAsync_ReturnsUsers()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var notAFriend = await environment.GenerateUniqueUserAsync();
			var archNemesis = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, notAFriend, archNemesis);

			// Act
			var blockedUsers = await director.GetBlockedUsersAsync(user.Id);

			// Assert
			Assert.Equal(2, blockedUsers.Count);
		}

		[Fact]
		public async Task FollowUserAsync_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var stranger = await environment.GenerateUniqueUserAsync();

			// Act
			await director.FollowUserAsync(user.Id, stranger.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task FollowUserAsync_Blocked_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var action = director.FollowUserAsync(user.Id, enemy.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await action);
		}

		[Fact]
		public async Task UnfollowUserAsync_FollowedUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var weirdDynamics = await environment.GenerateUniqueUserAsync();
			await director.FollowUserAsync(user.Id, weirdDynamics.Id);

			// Act
			await director.UnfollowUserAsync(user.Id, weirdDynamics.Id);
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

		[Fact]
		public async Task RateUserAsync_Self_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var action = director.RateUserAsync(user.Id, user.Id, UserRating.Positive);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await action);
		}

		[Fact]
		public async Task RateUserAsync_Neutral_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			await director.RateUserAsync(user.Id, randomUser.Id, UserRating.Positive);
			await director.RateUserAsync(randomUser.Id, user.Id, UserRating.Positive);

			// Assert
			Assert.Equal(1, (await user.Ratings).Postitive);
			Assert.Equal(1, (await randomUser.Ratings).Postitive);
		}

		[Fact]
		public async Task RateUserAsync_OverwriteRating_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();
			await director.RateUserAsync(user.Id, randomUser.Id, UserRating.Positive);

			// Act
			await director.RateUserAsync(user.Id, randomUser.Id, UserRating.Negative);

			// Assert
			Assert.Equal(0, (await randomUser.Ratings).Postitive);
			Assert.Equal(1, (await randomUser.Ratings).Negative);
		}

		[Fact]
		public async Task RateUserAsync_MultipleRatings_Drops()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();
			await director.RateUserAsync(user.Id, randomUser.Id, UserRating.Positive);

			// Act
			await director.RateUserAsync(user.Id, randomUser.Id, UserRating.Positive);

			// Assert
			Assert.Equal(1, (await randomUser.Ratings).Postitive);
		}
	}
}
