using System;
using Core.Boundaries;
using System.Threading.Tasks;
using Core.Entities;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Shared;
using System.IO;
using System.Collections.Concurrent;

namespace Core.Tests.Entities
{
    public class EventTests : CoreTest
	{
		///////
		// Composition
		////////////////

		[Fact]
		public void ValidateAndNormalise_ValidEvent_ReturnsTrue()
		{
			// Arrange
			var validEvent = new Event
			{
				Name = "Valid Event",
				Description = "A valid event description",
				StartTime = DateTimeOffset.Now,
				GroupMinimum = 5,
				GroupMaximum = 10
			};

			// Act
			bool result = validEvent.ValidateAndNormalise();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void ValidateAndNormalise_InvalidEvent_ReturnsFalse()
		{
			// Arrange
			var invalidEvent = new Event
			{
				Name = "Invalid Event",
				Description = "A".PadLeft(Event.MaximumDescLength + 1),
				StartTime = DateTimeOffset.Now - TimeSpan.FromDays(8),
				GroupMinimum = 5,
				GroupMaximum = 2
			};

			// Act
			bool result = invalidEvent.ValidateAndNormalise();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task GetFriendsOf_ReturnsFriends()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var friend1 = await environment.GenerateUniqueUserAsync();
			var friend2 = await environment.GenerateUniqueUserAsync();
			var randomGuest = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend1, friend2);

			var @event = await environment.GenerateUpcomingEventAsync(host, user, friend1, friend2, randomGuest);

			// Act
			var friends = await @event.GetFriendsOf(friend2);

			// Assert
			Assert.Equal(2, friends.Count);
		}

		/////
		// Checks
		///////////

		[Fact]
		public async Task IsVisibleTo_Neutral_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = await @event.IsVisibleTo(user);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsVisibleTo_Blocked_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(host, user);

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = await @event.IsVisibleTo(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsJoinableTo_Neutral_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.UpdateUserLocationAsync(user, @event.Location.Latitude, @event.Location.Longitude);

			// Act
			var result = await @event.IsJoinableBy(user);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsJoinableTo_Blocked_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(host, user);

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = await @event.IsJoinableBy(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsModifiableBy_Host_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = @event.IsModifiableBy(host);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsModifiableBy_Guest_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host, guest);

			// Act
			var result = @event.IsModifiableBy(guest);

			// Assert
			Assert.False(result);
			
		}

		[Fact]
		public async Task IsModifiableBy_Neutral_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = @event.IsModifiableBy(user);

			// Assert
			Assert.False(result);
			
		}

		[Fact]
		public async Task IsHostedBy_Host_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = @event.IsHostedBy(host);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsHostedBy_Guest_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host, guest);

			// Act
			var result = @event.IsHostedBy(guest);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsHostedBy_Neutral_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = @event.IsHostedBy(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task HasUserRelationship_Host_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = await @event.HasUserRelationship(host);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasUserRelationship_Watching_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var watchingUser = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, watchingUser, EventBond.Watching);

			// Act
			var result = await @event.HasUserRelationship(watchingUser);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasUserRelationship_Incoming_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var incomingGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, incomingGuest, EventBond.Guest);

			// Act
			var result = await @event.HasUserRelationship(incomingGuest);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasUserRelationship_ActiveGuest_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, guest, EventBond.Arrived);

			// Act
			var result = await @event.HasUserRelationship(guest);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasUserRelationship_LeftGuest_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var leftGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, leftGuest, EventBond.Left);

			// Act
			var result = await @event.HasUserRelationship(leftGuest);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasUserRelationship_KickedGuest_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var kickedGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, kickedGuest, EventBond.Kicked);

			// Act
			var result = await @event.HasUserRelationship(kickedGuest);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasUserRelationship_Neutral_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = await @event.HasUserRelationship(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task WasAttendedBy_ActiveGuest_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, guest, EventBond.Arrived);

			// Act
			var result = await @event.WasAttendedBy(guest);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task WasAttendedBy_LeftGuest_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var leftGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, leftGuest, EventBond.Left);

			// Act
			var result = await @event.WasAttendedBy(leftGuest);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task WasAttendedBy_KickedGuest_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var kickedGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, kickedGuest, EventBond.Kicked);

			// Act
			var result = await @event.WasAttendedBy(kickedGuest);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task WasAttendedBy_Neutral_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var result = await @event.WasAttendedBy(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsInRange_CloseUser_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.UpdateUserLocationAsync(user,
				@event.Location.Latitude,
				@event.Location.Longitude);

			// Act
			var result = await @event.IsInRange(user);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsInRange_FarUser_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.UpdateUserLocationAsync(user,
				@event.Location.Latitude + 15,
				@event.Location.Longitude + 15);

			// Act
			var result = await @event.IsInRange(user);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsStartable_Waiting_ReturnsTrue()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = environment.CreateTestEvent(host);
			@event.StartTime = Psijic.Time;
			@event = await environment.GenerateEventUnsafeAsync(@event, host);

			await environment.SetEventState(@event, EventState.Upcoming);
			await environment.UpdateUserLocationAsync(host,
				@event.Location.Latitude,
				@event.Location.Longitude);

			// Act
			var result = await @event.IsStartable();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsStartable_Started_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateOngoingEventAsync(host);

			// Act
			var result = await @event.IsStartable();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsStartable_HostFar_ReturnsFalse()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			await environment.UpdateUserLocationAsync(host,
				@event.Location.Latitude + 15,
				@event.Location.Longitude + 15);

			// Act
			var result = await @event.IsStartable();

			// Assert
			Assert.False(result);
		}

		//////
		// Effects
		////////////

		[Fact]
		public async Task Started_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			await @event.Started();
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task Ended_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			await @event.Ended();
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task Etched_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			await @event.Etched(host);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task Reported_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			await @event.Reported();
			// If no exception is thrown, the test is successful
		}

		//////
		// Actions
		////////////

		[Fact]
		public async Task NotifyActive_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var incomingUser = await environment.GenerateUniqueUserAsync();
			var activeGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, incomingUser, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, activeGuest, EventBond.Arrived);

			await environment.SubscribeUserAsync(incomingUser, DeviceType.iOS, incomingUser.Id.ToString());
			await environment.SubscribeUserAsync(activeGuest, DeviceType.iOS, activeGuest.Id.ToString());
			string notificationTitle = "event title", notificationMessage = "message test";

			// Act
			await @event.NotifyActive(notificationTitle, notificationMessage);

			// Assert
			var incomingUserMessages = environment.GetUserMessages(incomingUser);
			Assert.Single(incomingUserMessages);

			var notification = incomingUserMessages[0];
			Assert.Equal(notificationTitle, notification.Title);
			Assert.Equal(notificationMessage, notification.Message);

			var guestMessages = environment.GetUserMessages(activeGuest);
			Assert.Single(guestMessages);

			notification = guestMessages[0];
			Assert.Equal(notificationTitle, notification.Title);
			Assert.Equal(notificationMessage, notification.Message);
		}

		[Fact]
		public async Task NotifyGuests_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var incomingUser = await environment.GenerateUniqueUserAsync();
			var leftGuest = await environment.GenerateUniqueUserAsync();
			var activeGuest = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.AddUserToEventAsync(@event, incomingUser, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, leftGuest, EventBond.Left);
			await environment.AddUserToEventAsync(@event, activeGuest, EventBond.Arrived);

			await environment.SubscribeUserAsync(incomingUser, DeviceType.iOS, incomingUser.Id.ToString());
			await environment.SubscribeUserAsync(leftGuest, DeviceType.iOS, leftGuest.Id.ToString());
			await environment.SubscribeUserAsync(activeGuest, DeviceType.iOS, activeGuest.Id.ToString());
			string notificationTitle = "event title", notificationMessage = "message test";

			// Act
			await @event.NotifyGuests(notificationTitle, notificationMessage);

			// Assert
			var incomingUserMessages = environment.GetUserMessages(incomingUser);
			Assert.Empty(incomingUserMessages);

			var leftGuestMessages = environment.GetUserMessages(leftGuest);
			Assert.Empty(leftGuestMessages);

			var guestMessages = environment.GetUserMessages(activeGuest);
			Assert.Single(guestMessages);

			var notification = guestMessages[0];
			Assert.Equal(notificationTitle, notification.Title);
			Assert.Equal(notificationMessage, notification.Message);
		}
	}
}
