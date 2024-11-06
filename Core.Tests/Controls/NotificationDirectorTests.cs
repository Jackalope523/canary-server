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
			TelegramMessage message = TelegramMessage.UserAppreciated;
			string action = "action";
			int messageCount = 3;

			// Act
			for (int i = 0; i < messageCount; i++)
			{ await environment.SaveNoteAsync(user, noter, message, action); }
			
			// Assert
			var notes = await director.GetTelegramsAsync(user.Id);
			Assert.Equal(messageCount, notes.Count);
		}

		[Fact]
		public async Task PostNoteAsync_ValidUsers_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var noter = await environment.GenerateUniqueUserAsync();
			TelegramMessage message = TelegramMessage.UserAppreciated;
			string context = "action";

			// Act
			await director.PostTelegramAsync(user, noter, message, context);

			// Assert
			var notes = await director.GetTelegramsAsync(user.Id);
			Assert.Single(notes);
			Assert.Equal(message, notes[0].Message);
			Assert.Equal(context, notes[0].Context);
		}

		[Fact]
		public async Task PostNoteAsync_BlockedUsers_Drops()
		{
			// Arrange
			var userA = await environment.GenerateUniqueUserAsync();
			var userB = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(userA, userB);
			TelegramMessage message = TelegramMessage.UserAppreciated;
			string context = "action";

			// Act
			await director.PostTelegramAsync(userA, userB, message, context);

			// Assert
			var notes = await director.GetTelegramsAsync(userA.Id);
			Assert.Empty(notes);
		}
	}
}
