using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Xunit.Abstractions;


namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class NestStoreTests : IDisposable
    {
        private static EFCoreSentry sentry = new(Harbor.Flag.Development);
        private static EFCoreNestStore nestStore = new(Harbor.Flag.Development);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;
        public NestStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            UserFactory userFactory = new UserFactory();
            subject1 = userFactory.Create();
            subject2 = userFactory.Create();

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject1));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject2));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.UserLinks.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
        }

        [Fact]
        public async Task AppreciateUserAsync_SUCCESS()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;

            await nestStore.AppreciateUserAsync(subject1.Id, subject2.Id, time);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.SelfId == subject1.Id && l.OtherId == subject2.Id).Single());

            Assert.NotNull(link);
            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(time, link.Time);
            Assert.Equal(UserLink.UserLinkType.Appreciate, link.Type);
        }
        [Fact]
        public async Task UnappreciateUserAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.Appreciate);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            await nestStore.UnappreciateUserAsync(subject1.Id, subject2.Id);

            int count = sentry.ExecuteRead(ctx => ctx.UserLinks.Count());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task BlockUserAsync_SUCCESS()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;

            await nestStore.BlockUserAsync(subject1.Id, subject2.Id, time);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.SelfId == subject1.Id && l.OtherId == subject2.Id).Single());

            Assert.NotNull(link);
            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(time, link.Time);
            Assert.Equal(UserLink.UserLinkType.Block, link.Type);
        }
        [Fact]
        public async Task UnblockUserAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.Block);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            await nestStore.UnblockUserAsync(subject1.Id, subject2.Id);

            int numLinks = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.CountAsync());

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public async Task RateUserAsync_UP()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;

            await nestStore.RateUserAsync(subject1.Id, subject2.Id, UserRating.Positive, time);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Single());

            Assert.NotNull(link);
            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(time, link.Time);
            Assert.Equal(UserLink.UserLinkType.RateUp, link.Type);
        }
        [Fact]
        public async Task RateUserAsync_Down()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;

            await nestStore.RateUserAsync(subject1.Id, subject2.Id, UserRating.Negative, time);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Single());

            Assert.NotNull(link);
            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(time, link.Time);
            Assert.Equal(UserLink.UserLinkType.RateDown, link.Type);
        }
        [Fact]
        public async Task RemoveUserRatingAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.RateUp);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            await nestStore.RemoveUserRatingAsync(subject1.Id, subject2.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetAppreciatedUsersAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.Appreciate);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.AddAsync(link));

            UserSilhouette user = (await nestStore.GetAppreciatedUsersAsync(subject1.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetBlockedUsersAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.Block);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(link));

            UserSilhouette user = (await nestStore.GetBlockedUsersAsync(subject1.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetUserRatingsAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.RateUp);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(link));          

            (int up,int down) = await nestStore.GetUserRatingsAsync(subject2.Id);

            Assert.Equal(1, up);
            Assert.Equal(0, down);
        }
        [Fact]
        public async Task GetCompanionsAsync_SUCCESS()
        {
            UserLinkFactory factory = new UserLinkFactory();
            UserLink link1 = factory.Create(subject1, subject2, UserLink.UserLinkType.Appreciate);
            UserLink link2 = factory.Create(subject2, subject1, UserLink.UserLinkType.Appreciate);

            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link1));
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link2));

            UserSilhouette user = (await nestStore.GetCompanionsAsync(subject1.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetUsersAppreciatingAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject2, subject1, UserLink.UserLinkType.Appreciate);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            UserSilhouette user = (await nestStore.GetUsersAppreciatingAsync(subject1.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetUsersBlockingAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject2, subject1, UserLink.UserLinkType.Block);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            UserSilhouette user = (await nestStore.GetUsersBlockingAsync(subject1.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
    }
}