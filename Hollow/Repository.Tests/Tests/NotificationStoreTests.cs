using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Shared;
using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class NotificationStoreTests: IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private static EFCoreSentry sentry = new(Harbor.Flag.Development);
        private static NotificationStore store = new NotificationStore(sentry);

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
            sentry.ExecuteWrite(ctx => ctx.Notes.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Subscriptions.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
        }

        [Fact]
        public async Task GetNotesAsync_SUCCESS()
        {
            Entities.Note note = new NoteFactory().Create(subject1, subject2);
            sentry.ExecuteWrite(ctx => ctx.Notes.Add(note));

            Core.Boundaries.Note found = (await store.GetNotesAsync(subject2.Id)).Single();

            Assert.NotNull(found);
            Assert.Equal(subject1.Id, found.NotifierId);
            Assert.Equal(note.Time, found.Time);
            Assert.Equal(note.Message, found.Message);
            Assert.Equal(note.Action, found.Action);
        }
        [Fact]
        public async Task GetUserSubscriptionAsync_SUCCESS()
        {
            Subscription subscription = new SubscriptionFactory().Create(subject1);
            sentry.ExecuteWrite(ctx => ctx.Subscriptions.Add(subscription));

            DeviceSilhouette device = await store.GetUserSubscriptionAsync(subject1.Id);

            Assert.NotNull(device);
            Assert.Equal(subscription.DeviceType, device.DeviceType);
            Assert.Equal(subscription.DeviceToken, device.DeviceToken);
        }
        [Fact]
        public async Task SaveNoteAsync_SUCCESS()
        {
            DateTimeOffset time = DateTimeOffset.MinValue;
            string message = "message";
            string action = "action";

            await store.SaveNoteAsync(subject2.Id, subject1.Id, DateTimeOffset.MinValue, message, action);

            Entities.Note saved = sentry.ExecuteRead(ctx => ctx.Notes.Single());

            Assert.NotNull(saved);
            Assert.Equal(subject1.Id, saved.NotifierId);
            Assert.Equal(subject2.Id, saved.RecipientId);
            Assert.Equal(time, saved.Time);
            Assert.Equal(message, saved.Message);
            Assert.Equal(action, saved.Action);
            Assert.False(saved.Read);
        }
        [Fact]
        public async Task SubscribeUserAsync_SUCCESS()
        {
            DeviceType type = DeviceType.Android;
            string token = "token";

            await store.SubscribeUserAsync(subject1.Id, type, token);

            Subscription saved = sentry.ExecuteRead(ctx => ctx.Subscriptions.Single());

            Assert.NotNull(saved);
            Assert.Equal(subject1.Id, saved.UserId);
            Assert.Equal(type, saved.DeviceType);
            Assert.Equal(token, saved.DeviceToken);
        }
        [Fact]
        public async Task UnsubscribeUserAsync_SUCCESS()
        {
            Subscription subscription = new SubscriptionFactory().Create(subject1);
            sentry.ExecuteWrite(ctx => ctx.Subscriptions.Add(subscription)); 

            await store.UnsubscribeUserAsync(subject1.Id);

            int count = sentry.ExecuteRead(ctx => ctx.Subscriptions.Count());

            Assert.Equal(0, count);
        }
    }
}
