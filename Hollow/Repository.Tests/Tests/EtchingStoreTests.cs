using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests.Tests
{
    public class EtchingStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static EtchingStore etchingStore = new EtchingStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject;
        private Event testEvent;
        public EtchingStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            subject = new UserFactory().Create();
            testEvent = new EventFactory().Create(subject);

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.PostLinks.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Posts.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Events.ExecuteDelete());
        }

        [Fact]
        public async Task AddEtchingAsync_SUCCESS()
        {
            DateTimeOffset postTime = DateTimeOffset.MinValue;
            string url = "URL";

            await etchingStore.AddEtchingAsync(testEvent.Id, subject.Id, postTime, url);

            Post created = await sentry.ExecuteReadAsync(ctx => ctx.Posts.FirstAsync());

            Assert.NotNull(created);
            Assert.Equal(subject.Id, created.OwnerId);
            Assert.Equal(testEvent.Id, created.EventId);
            Assert.Equal(postTime, created.PostedAt);
            Assert.Equal(url, created.PhotoURL);
        }
        [Fact]
        public async Task RemoveEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            await etchingStore.RemoveEtchingAsync(testEtching.Id);

            int numPosts = await sentry.ExecuteReadAsync(ctx => ctx.Posts.CountAsync());

            Assert.Equal(0, numPosts);
        }
        [Fact]
        public async Task GetEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            Etching retrieved = await etchingStore.GetEtchingAsync(testEtching.Id);

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
        }
        [Fact]
        public async Task GetEtchingsByUserAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            int a = sentry.ExecuteRead(ctx => ctx.Posts.Count());
            _testOutputHelper.WriteLine(a.ToString());

           Etching retrieved = (await etchingStore.GetEtchingsByUserAsync(subject.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
        }
        [Fact]
        public async Task GetEtchingsForEventAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            Etching retrieved = (await etchingStore.GetEtchingsForEventAsync(testEvent.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
        }
        [Fact]
        public async Task RateEtchingAsync_SUCCESS()
        {
            PostLink.PostLinkType rating = PostLink.PostLinkType.RateUp;
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            await etchingStore.RateEtchingAsync(testEtching.Id, subject.Id, Shared.UserRating.Positive);

            PostLink created = await sentry.ExecuteReadAsync(ctx => ctx.PostLinks.FirstAsync());

            Assert.Equal(subject.Id, created.SelfId);
            Assert.Equal(testEtching.Id, created.OtherId);
            Assert.Equal(rating, created.Type);
        }
        [Fact]
        public async Task RemoveEtchingRatingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            PostLink rating = new PostLinkFactory().Create(subject, testEtching);
            await sentry.ExecuteWriteAsync(ctx => ctx.PostLinks.AddAsync(rating));

            await etchingStore.RemoveEtchingRatingAsync(testEtching.Id, subject.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.PostLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GenerateFeedForUserAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}

