using System;
using Core.Boundaries;
using System.Threading.Tasks;
using Core.Entities;
using Xunit;
using System.IO;
using Shared;

namespace Core.Tests.Entities
{
    public class UserTests : CoreTest
    {
		///////
		// Composition
		////////////////

		[Fact]
		public void ValidateAndNormalise_ValidUser_ReturnsTrue()
		{
			// Arrange
			var validUser = new User
			{
				PhoneNumber = "+1234567890",
				Email = "user@example.com",
				DateOfBirth = DateTimeOffset.Now.AddYears(-25)
			};

			// Act
			bool result = validUser.ValidateAndNormalise();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void ValidateAndNormalise_InvalidUser_ReturnsFalse()
		{
			// Arrange
			var invalidUser = new User
			{
				PhoneNumber = "invalid",
				Email = "invalid_email",
				DateOfBirth = DateTimeOffset.Now.AddYears(-15)
			};

			// Act
			bool result = invalidUser.ValidateAndNormalise();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task CalculateReputation_NoChange_SameReputation()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var oldReputation = user.Reputation;

			// Act
			await user.CalculateReputation();

			// Assert
			Assert.Equal(oldReputation, user.Reputation);
		}

		[Fact]
		public async Task CalculateReputation_Changed_NewReputation()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var nemesis = await environment.GenerateUniqueUserAsync();

			var oldReputation = user.Reputation;
			await environment.Terminal.ProfileOperations.RateUserAsync(nemesis.Id, user.Id, UserRating.Negative);

			// Act
			await user.CalculateReputation();

			// Assert
			Assert.NotEqual(oldReputation, user.Reputation);
		}

		[Fact]
		public async Task CalculateCharacter_NewCharacter()
		{
			// Arrange
			var host = environment.CreateTestUser();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = environment.CreateTestEvent(host);
			@event.Character = new(new(100,100,100,100,100,100,100));
			var oldCharacter = user.Character;

			// Act
			user.CalculateCharacter(@event, TimeSpan.FromMinutes(45));

			// Assert
			Assert.NotEqual(oldCharacter, user.Character);
		}

		[Fact]
		public async Task NextEvent_HasEvent_ReturnsEvent()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(user);

			// Act
			var returnedEvent = await user.NextEvent();

			// Assert
			Assert.Equal(@event, returnedEvent);
		}

		[Fact]
		public async Task NextEvent_HasNone_ReturnsNoneEvent()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var returnedEvent = await user.NextEvent();

			// Assert
			Assert.Equal(Event.None, returnedEvent);
		}

		/////
		// Checks
		///////////

		[Fact]
		public async Task IsFriendsWith_Friend_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			// Act
			var isFriends = await user.IsFriendsWith(friend);

			// Assert
			Assert.True(isFriends);
		}

		[Fact]
		public async Task IsFriendsWith_Neutral_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var isFriends = await user.IsFriendsWith(randomUser);

			// Assert
			Assert.False(isFriends);
		}

		[Fact]
		public async Task IsFollowing_FollowedUser_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			// Act
			var isFollowing = await user.IsFollowing(friend);

			// Assert
			Assert.True(isFollowing);
		}

		[Fact]
		public async Task IsFollowing_Neutral_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var isFollowing = await user.IsFollowing(randomUser);

			// Assert
			Assert.False(isFollowing);
		}

		[Fact]
		public async Task IsBlocking_BlockedUser_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var isBlocking = await user.IsBlocking(enemy);

			// Assert
			Assert.True(isBlocking);
		}

		[Fact]
		public async Task IsBlocking_Neutral_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var isBlocking = await user.IsBlocking(randomUser);

			// Assert
			Assert.False(isBlocking);
		}

		[Fact]
		public async Task IsBlockedBy_Blocked_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var enemy = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, enemy);

			// Act
			var isBlocking = await user.IsBlocking(enemy);

			// Assert
			Assert.True(isBlocking);
		}

		[Fact]
		public async Task IsBlockedBy_Neutral_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var randomUser = await environment.GenerateUniqueUserAsync();

			// Act
			var isBlocking = await user.IsBlocking(randomUser);

			// Assert
			Assert.False(isBlocking);
		}

		[Fact]
		public async Task IsAtEvent_AtEvent_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateOngoingEventAsync(host);
			await environment.AddUserToEventAsync(@event, user, EventBond.Arrived);

			// Act
			var isAtEvent = await user.IsAtEvent();

			// Assert
			Assert.True(isAtEvent);
		}

		[Fact]
		public async Task IsAtEvent_Sload_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var isAtEvent = await user.IsAtEvent();

			// Assert
			Assert.False(isAtEvent);
		}

		[Fact]
		public async Task CanView_ValidEvent_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var canView = await user.CanView(@event);

			// Assert
			Assert.True(canView);
		}

		[Fact]
		public async Task CanView_Blocked_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, host);
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var canView = await user.CanView(@event);

			// Assert
			Assert.False(canView);
		}

		[Fact]
		public async Task CanView_Limited_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			await environment.UpdateUser(user, nameof(UserShard.AccountStatus), UserAccountStatus.Limited);
			user = new(await environment.Terminal.AccountDatabase.FindUserByIdAsync(user.Id));

			// Act
			var canView = await user.CanView(@event);

			// Assert
			Assert.False(canView);
		}

		[Fact]
		public async Task CanView_FriendLimited_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await environment.UpdateUser(user, nameof(User.AccountStatus), UserAccountStatus.Limited);
			
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.ForceFriendshipAsync(user, host);

			// Act
			var canView = await user.CanView(@event);

			// Assert
			Assert.True(canView);
		}

		[Fact]
		public async Task CanJoin_JoinableEventAndVisibleUser_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.UpdateUserLocationAsync(user, @event.Location.Latitude, @event.Location.Longitude);

			// Act
			var canJoin = await user.CanJoin(@event);

			// Assert
			Assert.True(canJoin);
		}

		[Fact]
		public async Task Etched_OwnedEtching_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(user);
			var etching = await environment.GenerateEtchingAsync(@event, user);

			// Act
			var etched = user.Etched(etching);

			// Assert
			Assert.True(etched);
		}

		[Fact]
		public async Task Etched_UnownedEtching_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			var etching = await environment.GenerateEtchingAsync(@event, host);

			// Act
			var etched = user.Etched(etching);

			// Assert
			Assert.False(etched);
		}

		[Fact]
		public async Task CanReport_Chill_ReturnsTrue()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var canReport = await user.CanReport();

			// Assert
			Assert.True(canReport);
		}

		[Fact]
		public async Task CanReport_Volatile_ReturnsFalse()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var canReport = await user.CanReport();

			// Assert
			Assert.True(canReport);
		}

		//////
		// Effects
		////////////

		[Fact]
		public async Task HandleHaunt_Unmoved_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await user.HandleHaunt();
			var oldHaunt = await user.Haunt;

			// Act
			await user.HandleHaunt();

			// Assert
			Assert.Equal(oldHaunt, await user.Haunt);
		}

		[Fact]
		public async Task HandleHaunt_Moved_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await user.HandleHaunt();
			var oldHaunt = await user.Haunt;

			user.LastKnownLocation.Set(new()
			{
				Latitude = ((await user.LastKnownLocation).Latitude + 1) / 2,
				Longitude = ((await user.LastKnownLocation).Longitude + 1) / 2
			});

			// Act
			await user.HandleHaunt();

			// Assert
			Assert.NotEqual(oldHaunt, await user.Haunt);
		}

		[Fact]
		public async Task Penalised_NewReputation()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var oldReputation = user.Reputation;
			await environment.Terminal.DisciplineDirector.PenaliseUserAsync(user, PenaltyType.Unreliable, Psijic.Time);
			await environment.Terminal.DisciplineDirector.PenaliseUserAsync(user, PenaltyType.Unreliable, Psijic.Time);

			// Act
			await user.Penalised();

			// Assert
			Assert.NotEqual(oldReputation, user.Reputation);
		}

		//////
		// Actions
		////////////

		[Fact]
		public async Task PostNote_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var noter = await environment.GenerateUniqueUserAsync();
			string message = "message", action = "action";

			// Act
			await user.PostNote(noter, message, action);

			// Assert
			var notes = await environment.GetNotesAsync(user);
			Assert.Single(notes);
			Assert.Equal(message, notes[0].Message);
			Assert.Equal(action, notes[0].Action);
		}

		[Fact]
		public async Task Notify_SubscribedUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await environment.SubscribeUserAsync(user, DeviceType.iOS, user.Id.ToString());
			string notificationTitle = "title", notificationBody = "body";

			// Act
			await user.Notify(notificationTitle, notificationBody);

			// Assert
			Assert.True(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));

			var userMessages = environment.GetUserMessages(user);
			Assert.Single(userMessages);

			var notification = userMessages[0];
			Assert.Equal(notificationTitle, notification.Title);
			Assert.Equal(notificationBody, notification.Message);
		}

		[Fact]
		public async Task Notify_UnsubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			await user.Notify("", "");

            // Assert
            Assert.False(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));
		}

		[Fact]
		public async Task NotifyFollowers_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var friend1 = await environment.GenerateUniqueUserAsync();
			var friend2 = await environment.GenerateUniqueUserAsync();

			await environment.ForceFriendshipAsync(user, friend1, friend2);
			await environment.SubscribeUserAsync(friend1, DeviceType.iOS, friend1.Id.ToString());
			await environment.SubscribeUserAsync(friend2, DeviceType.iOS, friend2.Id.ToString());
			string notificationTitle = "event title", notificationMessage = "message test";

			// Act
			await user.NotifyFollowers(notificationTitle, notificationMessage);

			// Assert
			var incomingUserMessages = environment.GetUserMessages(friend1);
			Assert.Single(incomingUserMessages);

			var guestMessages = environment.GetUserMessages(friend2);
			Assert.Single(guestMessages);
		}
	}
}
