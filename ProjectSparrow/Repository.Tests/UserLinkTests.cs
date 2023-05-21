using Repository.Contexts;
using Repository.Entities;
using Repository.Sentries;
using Server.Boundaries;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static PhoneNumbers.PhoneNumber;

namespace Repository.Tests
{
    public class UserLinkTests : IDisposable
    {
        private static TestSentry sentry = TestSentry.GetTestSentry();
        private static QueryStore store = new QueryStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private string subject1PhoneNumber = "000-000-0000";
        private string subject1Email = "email_0@test.com";
        private string subject1NormalizedEmail = "email_0@test.com";
        private string subject1Name = "name";
        private string subject1SecurityStamp = "stamp";
        private DateTimeOffset subject1DateOfBirth = new DateTimeOffset(new DateTime(0));

        private User subject2;
        private string subject2PhoneNumber = "111-111-1111";
        private string subject2Email = "email_1@test.com";
        private string subject2NormalizedEmail = "email_1@test.com";
        private string subject2Name = "nome";
        private string subject2SecurityStamp = "Stampy";
        private DateTimeOffset subject2DateOfBirth = new DateTimeOffset(new DateTime(10));


        public UserLinkTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            subject1 = new User
            {
                PhoneNumber = subject1PhoneNumber,
                Email = subject1Email,
                NormalizedEmail = subject1NormalizedEmail,
                Name = subject1Name,
                SecurityStamp = subject1SecurityStamp,
                DateOfBirth = subject1DateOfBirth
            };

            subject2 = new User
            {
                PhoneNumber = subject2PhoneNumber,
                Email = subject2Email,
                NormalizedEmail = subject2NormalizedEmail,
                Name = subject2Name,
                SecurityStamp = subject2SecurityStamp,
                DateOfBirth = subject2DateOfBirth
            };

            sentry.GetContext().Users.Add(subject1);
            sentry.GetContext().Users.Add(subject2);
        }
        public void Dispose()
        {
            sentry.GetContext().Users.Remove(subject1);
            sentry.GetContext().Users.Remove(subject2);
            sentry.GetContext().SaveChanges();
        }

        [Fact]
        public void FollowUser_SUCCESS()
        {
            Guid id1 = sentry.GetContext().Users.Where(u => u.Name == subject1Name).Select(u => u.Id).Single();
            Guid id2 = sentry.GetContext().Users.Where(u => u.Name == subject2Name).Select(u => u.Id).Single();

            store.FollowUser(id1, id2);

            UserLink link = sentry.GetContext().UserLinks.Where(l => l.SelfId== id1 && l.OtherId== id2).Single();

            Assert.NotNull(link);
            Assert.Equal(UserLink.UserLinkType.Follow, link.Type);
        }
        [Fact]
        public void UnfollowUser_SUCCESS()
        {
            Guid id1 = sentry.GetContext().Users.Where(u => u.Name == subject1Name).Select(u => u.Id).Single();
            Guid id2 = sentry.GetContext().Users.Where(u => u.Name == subject2Name).Select(u => u.Id).Single();

            sentry.GetContext().UserLinks.Add(new UserLink { SelfId = id1, OtherId = id2, Type = UserLink.UserLinkType.Follow });
            sentry.GetContext().SaveChanges();

            store.UnfollowUser(id1, id2);

            int numLinks = sentry.GetContext().UserLinks.Count();

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public void BlockUser_SUCCESS()
        {
            Guid id1 = sentry.GetContext().Users.Where(u => u.Name == subject1Name).Select(u => u.Id).Single();
            Guid id2 = sentry.GetContext().Users.Where(u => u.Name == subject2Name).Select(u => u.Id).Single();

            store.BlockUser(id1, id2);

            UserLink link = sentry.GetContext().UserLinks.Where(l => l.SelfId == id1 && l.OtherId == id2).Single();

            Assert.NotNull(link);
            Assert.Equal(UserLink.UserLinkType.Block, link.Type);
        }
        [Fact]
        public void UnblockUser_SUCCESS()
        {
            Guid id1 = sentry.GetContext().Users.Where(u => u.Name == subject1Name).Select(u => u.Id).Single();
            Guid id2 = sentry.GetContext().Users.Where(u => u.Name == subject2Name).Select(u => u.Id).Single();

            sentry.GetContext().UserLinks.Add(new UserLink { SelfId = id1, OtherId = id2, Type = UserLink.UserLinkType.Block });
            sentry.GetContext().SaveChanges();

            store.UnblockUser(id1, id2);

            int numLinks = sentry.GetContext().UserLinks.Count();

            Assert.Equal(0, numLinks);
        }
        [Fact]
        public void RateUser_UP()
        {
            Guid id1 = sentry.GetContext().Users.Where(u => u.Name == subject1Name).Select(u => u.Id).Single();
            Guid id2 = sentry.GetContext().Users.Where(u => u.Name == subject2Name).Select(u => u.Id).Single();

            store.RateUser(id1, id2, Shared.UserRating.Positive);

            UserLink link = sentry.GetContext().UserLinks.Single();

            Assert.Equal(id1, link.SelfId);
            Assert.Equal(id2, link.OtherId);
            Assert.Equal(UserLink.UserLinkType.RateUp, link.Type);
        }
        [Fact]
        public void RateUser_Down()
        {
            Guid id1 = sentry.GetContext().Users.Where(u => u.Name == subject1Name).Select(u => u.Id).Single();
            Guid id2 = sentry.GetContext().Users.Where(u => u.Name == subject2Name).Select(u => u.Id).Single();

            store.RateUser(id1, id2, Shared.UserRating.Negative);

            UserLink link = sentry.GetContext().UserLinks.Single();

            Assert.Equal(id1, link.SelfId);
            Assert.Equal(id2, link.OtherId);
            Assert.Equal(UserLink.UserLinkType.RateDown, link.Type);
        }
    }
}