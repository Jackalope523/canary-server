using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
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

			var hostedEvent = await environment.GeneratePastEventAsync(user);
			var attendedEvent = await environment.GeneratePastEventAsync(host, user);
			var unattendedEvent = await environment.GeneratePastEventAsync(host);
			var ongoingEvent = await environment.GenerateUpcomingEventAsync(host, user);

			var funLovingEtching = await environment.GenerateEtchingAsync(attendedEvent, user);
			var lessLovingEtching = await environment.GenerateEtchingAsync(attendedEvent, host);
			var okEtching = await environment.GenerateEtchingAsync(ongoingEvent, user);

			// Act
			var (Events, Etchings) = await director.GetUserNestAsync(user.Id, user.Id);

			// Assert
			Assert.Equal(3, Events.Count);
			Assert.Equal(hostedEvent.ToEventShard(), Events.Find(e => e.Id.Equals(hostedEvent.Id)));
			Assert.Equal(attendedEvent.ToEventShard(), Events.Find(e => e.Id.Equals(attendedEvent.Id)));
			Assert.Equal(ongoingEvent.ToEventShard(), Events.Find(e => e.Id.Equals(ongoingEvent.Id)));

			Assert.Equal(3, Etchings.Count);
			Assert.Equal(funLovingEtching, Etchings.Find(e => e.Id.Equals(funLovingEtching.Id)));
			Assert.Equal(lessLovingEtching, Etchings.Find(e => e.Id.Equals(lessLovingEtching.Id)));
			Assert.Equal(okEtching, Etchings.Find(e => e.Id.Equals(okEtching.Id)));
		}

		[Fact]
		public async Task GetUserNestAsync_Friend_ReturnsNest()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var hostedEvent = await environment.GeneratePastEventAsync(user, friend);
			var mutuallyAttendedEvent = await environment.GeneratePastEventAsync(host, user, friend);
			var unattendedEvent = await environment.GeneratePastEventAsync(friend);
			var ongoingEvent = await environment.GenerateUpcomingEventAsync(host, friend);

			var userEtching = await environment.GenerateEtchingAsync(hostedEvent, user);
			var friendEtching = await environment.GenerateEtchingAsync(mutuallyAttendedEvent, friend);
			var hostEtching = await environment.GenerateEtchingAsync(mutuallyAttendedEvent, host);
			var ongoingEventFriendEtching = await environment.GenerateEtchingAsync(ongoingEvent, friend);
			var unattendedEventFriendEtching = await environment.GenerateEtchingAsync(unattendedEvent, friend);

			// Act
			var (Events, Etchings) = await director.GetUserNestAsync(user.Id, friend.Id);

			// Assert
			Assert.Equal(4, Events.Count);

			Assert.Equal(3, Etchings.Count);
		}

		[Fact]
		public async Task GetUserNestAsync_NeutralHost_ReturnsPublicNest()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			var mutuallyAttendedEvent = await environment.GeneratePastEventAsync(user, randomUser);
			var unattendedEvent = await environment.GeneratePastEventAsync(randomUser);

			var mutualEventEtching = await environment.GenerateEtchingAsync(mutuallyAttendedEvent, randomUser);
			var unattendedEventEtching = await environment.GenerateEtchingAsync(unattendedEvent, randomUser);

			// Act
			var (Events, Etchings) = await director.GetUserNestAsync(user.Id, randomUser.Id);

			// Assert
			Assert.Equal(2, Events.Count);

			Assert.Single(Etchings);
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
		public async Task GetUserActivityAsync_Self_ReturnsActivity()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();

			var pastEvent = await environment.GeneratePastEventAsync(user);
			var ongoingEvent = await environment.GenerateOngoingEventAsync(user);
			var upcomingEvent = await environment.GenerateUpcomingEventAsync(host, user);
			var anotherUpcomingEvent = await environment.GenerateUpcomingEventAsync(user);

			// Act
			var activity = await director.GetUserActivityAsync(user.Id, user.Id);

			// Assert
			Assert.Equal(3, activity.Count);
		}

		[Fact]
		public async Task GetUserActivityAsync_Friend_ReturnsActivity()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var randomHost = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var pastEvent = await environment.GeneratePastEventAsync(friend);
			var ongoingEvent = await environment.GenerateOngoingEventAsync(friend);
			var upcomingEvent = await environment.GenerateUpcomingEventAsync(user, friend);
			var anotherUpcomingEvent = await environment.GenerateUpcomingEventAsync(randomHost, friend);
			var yetAnotherUpcomingEvent = await environment.GenerateUpcomingEventAsync(friend);

			// Act
			var activity = await director.GetUserActivityAsync(user.Id, friend.Id);

			// Assert
			Assert.Equal(4, activity.Count);
		}

		[Fact]
		public async Task GetUserActivityAsync_Neutral_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var activity = director.GetUserActivityAsync(user.Id, randomUser.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await activity);
		}

		[Fact]
		public async Task GetUserActivityAsync_Blocked_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var activity = director.GetUserActivityAsync(user.Id, enemy.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await activity);
		}

		[Fact]
		public async Task GetFriendActivityAsync_ReturnsFriendActivity()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var activeFriend = await environment.GenerateUniqueUserAsync();
			var sloadButChill = await environment.GenerateUniqueUserAsync();
			var randomHost = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, activeFriend, sloadButChill);

			var pastEvent = await environment.GeneratePastEventAsync(activeFriend);
			var ongoingEvent = await environment.GenerateOngoingEventAsync(activeFriend, sloadButChill);
			var upcomingEvent = await environment.GenerateUpcomingEventAsync(user, activeFriend);
			var anotherUpcomingEvent = await environment.GenerateUpcomingEventAsync(randomHost, activeFriend);
			var yetAnotherUpcomingEvent = await environment.GenerateUpcomingEventAsync(activeFriend);

			// Act
			var activity = await director.GetFriendActivityAsync(user.Id);

			// Assert
			Assert.Equal(2, activity.Keys.Count);
			Assert.Equal(4, activity[activeFriend.ToUserSilhouette()].Count);
			Assert.Single(activity[sloadButChill.ToUserSilhouette()]);
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
