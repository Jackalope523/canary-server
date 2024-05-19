using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class EtchingStoreTests : IDisposable
    {
        private static EFCoreSentry sentry = new(Harbor.Flag.Development);
        private static EFCoreEtchingStore etchingStore = new(Harbor.Flag.Development);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject;
        private Gathering testGathering;
        public EtchingStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            subject = new UserFactory().Create();
            testGathering = new GatheringFactory().Create(subject);

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));
            sentry.ExecuteWrite(ctx => ctx.Gatherings.Add(testGathering));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.PostLinks.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Posts.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Gatherings.ExecuteDelete());
        }

        [Fact]
        public async Task AddEtchingAsync_SUCCESS()
        {
            DateTimeOffset postTime = DateTimeOffset.MinValue;
            string url = "URL";

            await etchingStore.AddEtchingAsync(testGathering.Id, subject.Id, postTime);

            Post created = await sentry.ExecuteReadAsync(ctx => ctx.Posts.FirstAsync());

            Assert.NotNull(created);
            Assert.Equal(subject.Id, created.OwnerId);
            Assert.Equal(testGathering.Id, created.GatheringId);
            Assert.Equal(postTime, created.PostedAt);
            Assert.Equal(url, created.PhotoURL);
            Assert.False(created.IsHidden);
        }
        [Fact]
        public async Task RemoveEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            await etchingStore.RemoveEtchingAsync(testEtching.Id);

            int numPosts = await sentry.ExecuteReadAsync(ctx => ctx.Posts.CountAsync());

            Assert.Equal(0, numPosts);
        }
        [Fact]
        public async Task GetEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            EtchingShard retrieved = await etchingStore.GetEtchingAsync(testEtching.Id);

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.User.Id);
            Assert.Equal(testEtching.GatheringId, retrieved.GatheringId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.IsHidden, retrieved.IsHidden);
        }
        [Fact]
        public async Task GetEtchingsByUserAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            int a = sentry.ExecuteRead(ctx => ctx.Posts.Count());
            _testOutputHelper.WriteLine(a.ToString());

           EtchingShard retrieved = (await etchingStore.GetEtchingsByUserAsync(subject.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.User.Id);
            Assert.Equal(testEtching.GatheringId, retrieved.GatheringId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.IsHidden, retrieved.IsHidden);
        }
        [Fact]
        public async Task GetEtchingsForGatheringAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            EtchingShard retrieved = (await etchingStore.GetEtchingsForGatheringAsync(testGathering.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.User.Id);
            Assert.Equal(testEtching.GatheringId, retrieved.GatheringId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.IsHidden, retrieved.IsHidden);
        }
        [Fact]
        public async Task RateEtchingAsync_SUCCESS()
        {
            PostLink.PostLinkType rating = PostLink.PostLinkType.RateUp;
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            await etchingStore.RateEtchingAsync(testEtching.Id, subject.Id, UserRating.Positive);

            PostLink created = await sentry.ExecuteReadAsync(ctx => ctx.PostLinks.FirstAsync());

            Assert.Equal(subject.Id, created.UserId);
            Assert.Equal(testEtching.Id, created.PostId);
            Assert.Equal(rating, created.Type);
        }
        [Fact]
        public async Task RemoveEtchingRatingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            PostLink rating = new PostLinkFactory().Create(subject, testEtching);
            await sentry.ExecuteWriteAsync(ctx => ctx.PostLinks.AddAsync(rating));

            await etchingStore.RemoveEtchingRatingAsync(testEtching.Id, subject.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.PostLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task HideEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testGathering);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            await etchingStore.HideEtchingAsync(testEtching.Id);

            EtchingShard retrieved = (await etchingStore.GetEtchingsForGatheringAsync(testGathering.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.User.Id);
            Assert.Equal(testEtching.GatheringId, retrieved.GatheringId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.NotEqual(testEtching.IsHidden, retrieved.IsHidden);
            Assert.True(retrieved.IsHidden);
        }
    }
}

