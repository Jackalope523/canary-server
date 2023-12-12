using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests
{
    public class ProfileStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static ProfileStore profileStore = new ProfileStore(sentry);
        private static EventStore store = new EventStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;

        private User testUser;
        private Event testEvent;


        public ProfileStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            UserFactory userFactory = new UserFactory();
            subject1 = userFactory.Create();
            subject2 = userFactory.Create();

            sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject1));
            sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject2));

            testUser = new UserFactory().Create();
            sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(testUser));

            testEvent = new EventFactory().Create();
            testEvent.HostId = testUser.Id;
            sentry.ExecuteWriteAsync(ctx => ctx.Events.Add(testEvent));
        }
        public void Dispose()
        {
            sentry.ExecuteWriteAsync(ctx => ctx.UserLinks.ExecuteDelete());
            sentry.ExecuteWriteAsync(ctx => ctx.Users.ExecuteDelete());

            sentry.ExecuteWriteAsync(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWriteAsync(ctx => ctx.Events.ExecuteDelete());
            sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.ExecuteDelete());
        }

        [Fact]
        public async Task FollowUserAsync_SUCCESS()
        {
            await profileStore.FollowUserAsync(subject1.Id, subject2.Id);

            UserLink link = await sentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.SelfId== subject1.Id && l.OtherId== subject2.Id).SingleAsync());

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
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetFollowedUsersAsync_SUCCESS() 
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetBlockedUsersAsync_SUCCESS() 
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetUserRatingsAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetFriendsAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetGuestListAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.Add(new EventLink { SelfId = testUser.Id, OtherId = testEvent.Id, Type = EventLink.EventLinkType.Attend }));

            List<UserSilhouette> guestList = await store.GetGuestListAsync(testEvent.Id);

            Assert.Single(guestList);
            Assert.Equal(testUser.Id, guestList.First().Id);
            Assert.Equal(testUser.Name, guestList.First().Name);
        }
        [Fact]
        public async Task AddUserToEventAsync_SUCCESS()
        {
            await store.AddUserToEventAsync(testUser.Id, testEvent.Id);

            EventLink link = await sentry.ExecuteReadAsync(ctx => ctx.EventLinks.FirstAsync());

            Assert.NotNull(link);
            Assert.Equal(testUser.Id, link.SelfId);
            Assert.Equal(testEvent.Id, link.OtherId);
            Assert.Equal(EventLink.EventLinkType.Attend, link.Type);
        }
        [Fact]
        public async Task RemoveUserFromEventAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.Add(new EventLink { SelfId = testUser.Id, OtherId = testEvent.Id, Type = EventLink.EventLinkType.Attend }));

            await store.RemoveUserFromEventAsync(testUser.Id, testEvent.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.EventLinks.CountAsync());

            Assert.Equal(0, count);
        }   
    }
}