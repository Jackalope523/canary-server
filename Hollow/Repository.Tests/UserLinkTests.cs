using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests
{
    public class UserLinkTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static ProfileStore store = new ProfileStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;
       

        public UserLinkTests(ITestOutputHelper testOutputHelper)
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
        public void FollowUser_SUCCESS()
        {
            store.FollowUser(subject1.Id, subject2.Id);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.SelfId== subject1.Id && l.OtherId== subject2.Id).Single());

            Assert.NotNull(link);
            Assert.Equal(UserLink.UserLinkType.Follow, link.Type);
        }
        [Fact]
        public void UnfollowUser_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(new UserLink { SelfId = subject1.Id, OtherId = subject2.Id, Type = UserLink.UserLinkType.Follow }));

            store.UnfollowUser(subject1.Id, subject2.Id);

            int numLinks = sentry.ExecuteRead(ctx => ctx.UserLinks.Count());

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public void BlockUser_SUCCESS()
        {
            store.BlockUser(subject1.Id, subject2.Id);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.SelfId == subject1.Id && l.OtherId == subject2.Id).Single());

            Assert.NotNull(link);
            Assert.Equal(UserLink.UserLinkType.Block, link.Type);
        }
        [Fact]
        public void UnblockUser_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(new UserLink { SelfId = subject1.Id, OtherId = subject2.Id, Type = UserLink.UserLinkType.Block }));

            store.UnblockUser(subject1.Id, subject2.Id);

            int numLinks = sentry.ExecuteRead(ctx => ctx.UserLinks.Count());

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public void RateUser_UP()
        {
            store.RateUser(subject1.Id, subject2.Id, Shared.UserRating.Positive);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Single());

            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(UserLink.UserLinkType.RateUp, link.Type);
        }
        [Fact]
        public void RateUser_Down()
        {
            store.RateUser(subject1.Id, subject2.Id, Shared.UserRating.Negative);

            UserLink link = sentry.ExecuteRead(ctx => ctx.UserLinks.Single());

            Assert.Equal(subject1.Id, link.SelfId);
            Assert.Equal(subject2.Id, link.OtherId);
            Assert.Equal(UserLink.UserLinkType.RateDown, link.Type);
        }
        [Fact]
        public void GetFollowedUsers_SUCCESS() 
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void GetBlockedUsers_SUCCESS() 
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void GetUserRatings_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void GetFriends_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}