using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class ProfileStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static ProfileStore profileStore = new ProfileStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;
        public ProfileStoreTests(ITestOutputHelper testOutputHelper)
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
        public async Task FollowUserAsync_SUCCESS()
        {
            await profileStore.FollowUserAsync(subject1.Id, subject2.Id);

            UserLink link = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.SelfId == subject1.Id && l.OtherId == subject2.Id).SingleAsync());

            Assert.NotNull(link);
            Assert.Equal(UserLink.UserLinkType.Follow, link.Type);
        }
        [Fact]
        public async Task UnfollowUserAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(new UserLink { SelfId = subject1.Id, OtherId = subject2.Id, Type = UserLink.UserLinkType.Follow }));

            await profileStore.UnfollowUserAsync(subject1.Id, subject2.Id);

            int numLinks = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.CountAsync());

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public async Task BlockUserAsync_SUCCESS()
        {
            await profileStore.BlockUserAsync(subject1.Id, subject2.Id);

            UserLink link = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.SelfId == subject1.Id && l.OtherId == subject2.Id).SingleAsync());

            Assert.NotNull(link);
            Assert.Equal(UserLink.UserLinkType.Block, link.Type);
        }
        [Fact]
        public async Task UnblockUserAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(new UserLink { SelfId = subject1.Id, OtherId = subject2.Id, Type = UserLink.UserLinkType.Block }));

            await profileStore.UnblockUserAsync(subject1.Id, subject2.Id);

            int numLinks = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.CountAsync());

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public async Task RateUserAsync_UP()
        {
            await profileStore.RateUserAsync(subject1.Id, subject2.Id, Shared.UserRating.Positive);

            UserLink link = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.SingleAsync());

            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(UserLink.UserLinkType.RateUp, link.Type);
        }
        [Fact]
        public async Task RateUserAsync_Down()
        {
            await profileStore.RateUserAsync(subject1.Id, subject2.Id, Shared.UserRating.Negative);

            UserLink link = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.SingleAsync());

            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(UserLink.UserLinkType.RateDown, link.Type);
        }
        [Fact]
        public async Task RemoveUserRatingAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.RateUp);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.AddAsync(link));

            await profileStore.RemoveUserRatingAsync(subject1.Id, subject2.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetFollowedUsersAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.Follow);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.AddAsync(link));

            UserSilhouette user = (await profileStore.GetFollowedUsersAsync(subject1.Id)).First();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetBlockedUsersAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.Block);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(link));

            UserSilhouette user = (await profileStore.GetBlockedUsersAsync(subject1.Id)).First();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetUserRatingsAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject1, subject2, UserLink.UserLinkType.RateUp);
            await sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Add(link));          

            (int up,int down) = await profileStore.GetUserRatingsAsync(subject2.Id);

            Assert.Equal(1, up);
            Assert.Equal(0, down);
        }
        [Fact]
        public async Task GetFriendsAsync_SUCCESS()
        {
            UserLinkFactory factory = new UserLinkFactory();
            UserLink link1 = factory.Create(subject1, subject2, UserLink.UserLinkType.Follow);
            UserLink link2 = factory.Create(subject2, subject1, UserLink.UserLinkType.Follow);

            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link1));
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link2));

            UserSilhouette user = (await profileStore.GetFriendsAsync(subject1.Id)).First();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetUsersFollowingAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject2, subject1, UserLink.UserLinkType.Follow);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            UserSilhouette user = (await profileStore.GetUsersFollowingAsync(subject1.Id)).First();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
        [Fact]
        public async Task GetUsersBlockingAsync_SUCCESS()
        {
            UserLink link = new UserLinkFactory().Create(subject2, subject1, UserLink.UserLinkType.Block);
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link));

            UserSilhouette user = (await profileStore.GetUsersBlockingAsync(subject1.Id)).First();

            Assert.NotNull(user);
            Assert.Equal(subject2.Id, user.Id);
            Assert.Equal(subject2.Name, user.Name);
        }
    }
}