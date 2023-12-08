using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
    public class NotificationDirectorTests : IAsyncLifetime
    {
        private TestEnvironment environment;
		private NotificationDirector director;

		private User testUser;

        public NotificationDirectorTests()
        {
            environment = new();
			director = environment.Terminal.NotificationDirector;
        }

		public async Task InitializeAsync()
		{
			testUser = await environment.GenerateTestUserAsync();
		}

		public Task DisposeAsync()
		{
			environment.Dispose();
			return Task.CompletedTask;
		}

		/////
		// Notes
		//////////

		[Fact]
		internal async Task GetNotesAsync_ReturnsNotes()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			string message = "message", action = "action";
			int messageCount = 3;

			// Act
			for (int i = 0; i < messageCount; i++)
			{ await environment.Terminal.NotificationDatabase.SaveNoteAsync(user.Id, testUser.Id, new DateTime(0), message, action); }
			
			// Assert
			var notes = await director.GetNotesAsync(user.Id);
			Assert.Equal(messageCount, notes.Count);
		}

		[Fact]
		internal async Task PostNoteAsync_ValidUsers_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			string message = "message", action = "action";

			// Act
			await director.PostNoteAsync(user, testUser, message, action);

			// Assert
			var notes = await director.GetNotesAsync(user.Id);
			Assert.Single(notes);
			Assert.Equal(message, notes[0].Message);
			Assert.Equal(action, notes[0].Action);
		}

		[Fact]
		internal async Task PostNoteAsync_BlockedUsers_Drops()
		{
			// Arrange
			var userA = await environment.GenerateTestUserAsync();
			var userB = await environment.GenerateTestUserAsync();
			await environment.Terminal.ProfileDirector.BlockUserAsync(userA.Id, userB.Id);
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
		internal async Task SubscribeUserAsync_ValidUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();

			// Act
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());

			// Assert
			var subscription = await environment.Terminal.NotificationDatabase.GetUserSubscriptionAsync(user.Id);
			Assert.False(string.IsNullOrEmpty(subscription.DeviceToken));
		}

		[Fact]
		internal async Task SubscribeUserAsync_SubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());

			// Act
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());
			// If no exception is thrown, the test is successful
		}

		[Fact]
		internal async Task UnsubscribeUserAsync_ValidUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());

			// Act
			await director.UnsubscribeUserAsync(user.Id);

			// Assert
			var subscription = await environment.Terminal.NotificationDatabase.GetUserSubscriptionAsync(user.Id);
			Assert.True(string.IsNullOrEmpty(subscription.DeviceToken));
		}

		[Fact]
		internal async Task UnsubscribeUserAsync_UnsubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();

			// Act
			await director.UnsubscribeUserAsync(user.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		internal async Task NotifyUserAsync_SubscribedUser_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			await director.SubscribeUserAsync(user.Id, DeviceType.iOS, user.Id.ToString());
			string notificationTitle = "title", notificationBody = "body";

			// Act
			await director.NotifyUserAsync(user, notificationTitle, notificationBody);

			// Assert
			Assert.True(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));

			ConcurrentBag<NotificationServiceStub.NotificationStub> userMessages;
			NotificationServiceStub.messages.TryGetValue(user.Id.ToString(), out userMessages);
			Assert.True(userMessages.Count == 1);

			NotificationServiceStub.NotificationStub notification;
			userMessages.TryTake(out notification);
			Assert.Equal(notificationTitle, notification.Title);
			Assert.Equal(notificationBody, notification.Message);
		}

		[Fact]
		internal async Task NotifyUserAsync_UnsubscribedUser_Drops()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();

			// Act
			await director.NotifyUserAsync(user, "", "");

			// Assert
			Assert.False(NotificationServiceStub.messages.ContainsKey(user.Id.ToString()));
		}
	}
}
