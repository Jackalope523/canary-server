using Core.Boundaries;
using Core.Controls;
using Core.Entities;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Controls
{
    public class NotificationDirectorTests : CoreTest
    {
		private NotificationDirector director;

        public NotificationDirectorTests()
        {
			director = environment.Terminal.NotificationDirector;
        }

		/////
		// Notes
		//////////

		[Fact]
		public async Task GetNotesAsync_ReturnsNotes()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var noter = await environment.GenerateUniqueUserAsync();
			string message = "message", action = "action";
			int messageCount = 3;

			// Act
			for (int i = 0; i < messageCount; i++)
			{ await environment.SaveNoteAsync(user, noter, message, action); }
			
			// Assert
			var notes = await director.GetNotesAsync(user.Id);
			Assert.Equal(messageCount, notes.Count);
		}

		[Fact]
		public async Task PostNoteAsync_ValidUsers_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var noter = await environment.GenerateUniqueUserAsync();
			string message = "message", action = "action";

			// Act
			await director.PostNoteAsync(user, noter, message, action);

			// Assert
			var notes = await director.GetNotesAsync(user.Id);
			Assert.Single(notes);
			Assert.Equal(message, notes[0].Message);
			Assert.Equal(action, notes[0].Action);
		}

		[Fact]
		public async Task PostNoteAsync_BlockedUsers_Drops()
		{
			// Arrange
			var userA = await environment.GenerateUniqueUserAsync();
			var userB = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(userA, userB);
			string message = "message", action = "action";

			// Act
			await director.PostNoteAsync(userA, userB, message, action);

			// Assert
			var notes = await director.GetNotesAsync(userA.Id);
			Assert.Empty(notes);
		}

		//////////
		// Push Notifications
		///////////////////////

		[Fact]
		public async Task SubscribeUserAsync_ValidUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());

			// Assert
			var subscription = await environment.GetUserSubscriptionAsync(user);
			Assert.False(string.IsNullOrEmpty(subscription.DeviceToken));
		}

		[Fact]
		public async Task SubscribeUserAsync_SubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());

			// Act
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task UnsubscribeUserAsync_ValidUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());

			// Act
			await director.UnsubscribeUserAsync(user.Id);

			// Assert
			var subscription = await environment.GetUserSubscriptionAsync(user);
			Assert.Null(subscription);
		}

		[Fact]
		public async Task UnsubscribeUserAsync_UnsubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			await director.UnsubscribeUserAsync(user.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task NotifyUserAsync_SubscribedUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());
			string notificationTitle = "title", notificationBody = "body";

			// Act
			await director.NotifyUserAsync(user, notificationTitle, notificationBody);

			// Assert
			Assert.True(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));

			var userMessages = environment.GetUserMessages(user);
			Assert.Single(userMessages);

			var notification = userMessages[0];
			Assert.Equal(notificationTitle, notification.Title);
			Assert.Equal(notificationBody, notification.Message);
		}

		[Fact]
		public async Task NotifyUserAsync_MultipleNotifications_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());
			string notificationTitle = "title", notificationBody = "body";

			// Act
			await director.NotifyUserAsync(user, notificationTitle, notificationBody);
			await director.NotifyUserAsync(user, notificationTitle, notificationBody);
			await director.NotifyUserAsync(user, notificationTitle, notificationBody);

			// Assert
			Assert.True(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));

			var userMessages = environment.GetUserMessages(user);
			Assert.Equal(3, userMessages.Count);
		}

		[Fact]
		public async Task NotifyUserAsync_UnsubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			await director.NotifyUserAsync(user, "", "");

			// Assert
			Assert.False(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));
		}
	}
}
