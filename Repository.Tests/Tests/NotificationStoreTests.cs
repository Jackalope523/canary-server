using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;

using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class NotificationStoreTests: IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private static EFCoreSentry sentry = new(Harbor.Flag.Development);
        private static EFCoreNotificationStore store = new(Harbor.Flag.Development);

        private User subject1;
        private User subject2;

        public NotificationStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            UserFactory factory = new UserFactory();
            subject1 = factory.Create();
            subject2 = factory.Create();

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject1));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject2));

        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.Telegrams.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Subscriptions.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
        }

        [Fact]
        public async Task GetNotesAsync_SUCCESS()
        {
            Telegram note = new NoteFactory().Create(subject1, subject2);
            sentry.ExecuteWrite(ctx => ctx.Telegrams.Add(note));

            TelegramShard found = (await store.GetTelegramsAsync(subject2.Id)).Single();

            Assert.NotNull(found);
            Assert.Equal(subject1.Id, found.NotifierId);
            Assert.Equal(note.Time, found.Time);
            Assert.Equal(note.Message, found.Message);
        }
        [Fact]
        public async Task SaveNoteAsync_SUCCESS()
        {
            DateTimeOffset time = DateTimeOffset.MinValue;
            TelegramMessage message = TelegramMessage.GatheringInvitation;
            string action = "action";

            await store.SaveTelegramAsync(subject2.Id, subject1.Id, DateTimeOffset.MinValue, message, action);

            Entities.Telegram saved = sentry.ExecuteRead(ctx => ctx.Telegrams.Single());

            Assert.NotNull(saved);
            Assert.Equal(subject1.Id, saved.NotifierId);
            Assert.Equal(subject2.Id, saved.RecipientId);
            Assert.Equal(time, saved.Time);
            Assert.Equal(message, saved.Message);
            Assert.Equal(action, saved.Action);
            Assert.False(saved.Read);
        }
        
    }
}
