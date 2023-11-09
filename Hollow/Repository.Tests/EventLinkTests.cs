using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Core.Boundaries;
using Xunit.Abstractions;



namespace Repository.Tests
{
    public class EventLinkTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static EventStore store = new EventStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private Guid testUserId;
        private string testUserPhoneNumber = "000-000-0000";
        private string testUserEmail = "email_0@test.com";
        private string testUserNormalizedEmail = "email_0@test.com";
        private string testUserName = "name";
        private string testUserSecurityStamp = "stamp";
        private DateTimeOffset subjectDateOfBirth = new DateTimeOffset(new DateTime(0));
        private User testUser;

        private Guid testEventId;
        private string testEventName = "event1";
        private string testEventDescription = "The first of many.";
        private string testEventEventType = "Proto";
        private DateTimeOffset testEventStartTime = new DateTimeOffset(new DateTime(0));
        private double testEventLatitude = 100;
        private double testEventLongitude = 100;
        private int testEventGroupMinimum = 0;
        private int testEventGroupMaximum = 1;
        private bool testIsEventOpen = false;
        private Event testEvent;

        public EventLinkTests(ITestOutputHelper testOutputHelper) 
        {
            _testOutputHelper = testOutputHelper;

            testUser = new User
            {
                PhoneNumber = testUserPhoneNumber,
                Email = testUserEmail,
                NormalisedEmail = testUserNormalizedEmail,
                Name = testUserName,
                SecurityStamp = testUserSecurityStamp,
                DateOfBirth = subjectDateOfBirth
            };

            sentry.ExecuteWrite(ctx => ctx.Users.Add(testUser));

            testUserId = testUser.Id;

            testEvent = new Event
            {
                HostId = Guid.Empty,
                Name = testEventName,
                Description = testEventDescription,
                StartTime = testEventStartTime,
                Location = new Point(testEventLongitude, testEventLatitude),
                GroupMinimum = testEventGroupMinimum,
                GroupMaximum = testEventGroupMaximum,
                IsEventOpen = testIsEventOpen
            };

            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));

            testEventId = testEvent.Id;
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Events.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.EventLinks.ExecuteDelete());
        }
        [Fact]
        public void GetGuestList_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(new EventLink { SelfId = testUserId, EventId = testEventId, Type = EventLink.EventLinkType.Attend }));

            List<UserSilhouette> guestList = store.GetGuestList(testEventId);

            Assert.Single(guestList);
            Assert.Equal(testUserId, guestList.First().Id);
            Assert.Equal(testUserName, guestList.First().Name);
        }
        [Fact]
        public void AddUserToEvent_SUCCESS() 
        {
            store.AddUserToEvent(testUserId, testEventId);

            EventLink link = sentry.ExecuteRead(ctx => ctx.EventLinks.First());

            Assert.NotNull(link);
            Assert.Equal(testUserId, link.SelfId);
            Assert.Equal(testEventId, link.EventId);
            Assert.Equal(EventLink.EventLinkType.Attend, link.Type);
        }
        [Fact]
        public void RemoveUserFromEvent_SUCCESS() 
        {
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(new EventLink { SelfId = testUserId, EventId = testEventId, Type = EventLink.EventLinkType.Attend }));

            store.RemoveUserFromEvent(testUserId, testEventId);

            int count = sentry.ExecuteRead(ctx => ctx.EventLinks.Count());

            Assert.Equal(0, count);
        }
        [Fact]
        public void FindUpcomingEvents_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void FindPastEvents_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void FindCurrentEvent_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}
