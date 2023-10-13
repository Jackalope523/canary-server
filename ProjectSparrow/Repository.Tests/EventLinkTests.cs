using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Repository.Entities;
using Repository.Sentries;
using Core.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Repository.Tests
{
    public class EventLinkTests : IDisposable
    {
        private static TestSentry sentry = TestSentry.GetTestSentry();
        private static QueryStore store = new QueryStore(sentry);

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
                NormalizedEmail = testUserNormalizedEmail,
                Name = testUserName,
                SecurityStamp = testUserSecurityStamp,
                DateOfBirth = subjectDateOfBirth
            };

            sentry.GetContext().Users.Add(testUser);
            sentry.GetContext().SaveChanges();

            testUserId = sentry.GetContext().Users.First().Id;

            testEvent = new Event
            {
                HostId = Guid.Empty,
                Name = testEventName,
                Description = testEventDescription,
                Type = testEventEventType,
                StartTime = testEventStartTime,
                Location = new Point(testEventLongitude, testEventLatitude),
                GroupMinimum = testEventGroupMinimum,
                GroupMaximum = testEventGroupMaximum,
                IsEventOpen = testIsEventOpen
            };

            sentry.GetContext().Events.Add(testEvent);
            sentry.GetContext().SaveChanges();

            testEventId = sentry.GetContext().Events.First().Id;
        }
        public void Dispose()
        {
            sentry.GetContext().Users.ExecuteDelete();
            sentry.GetContext().Events.ExecuteDelete();
            sentry.GetContext().EventLinks.ExecuteDelete();
        }
        [Fact]
        public void GetGuestList_SUCCESS()
        {
            sentry.GetContext().EventLinks.Add(new EventLink { SelfId = testUserId, EventId = testEventId, Type = EventLink.EventLinkType.Attend });
            sentry.GetContext().SaveChanges();

            List<UserSilhouette> guestList = store.GetGuestList(testEventId);

            Assert.Single(guestList);
            Assert.Equal(testUserId, guestList.First().Id);
            Assert.Equal(testUserName, guestList.First().Name);
        }
        [Fact]
        public void AddUserToEvent_SUCCESS() 
        {
            store.AddUserToEvent(testUserId, testEventId);

            EventLink link = sentry.GetContext().EventLinks.First();

            Assert.NotNull(link);
            Assert.Equal(testUserId, link.SelfId);
            Assert.Equal(testEventId, link.EventId);
            Assert.Equal(EventLink.EventLinkType.Attend, link.Type);
        }
        [Fact]
        public void RemoveUserFromEvent_SUCCESS() 
        {
            sentry.GetContext().EventLinks.Add(new EventLink { SelfId = testUserId, EventId = testEventId, Type = EventLink.EventLinkType.Attend });
            sentry.GetContext().SaveChanges();

            store.RemoveUserFromEvent(testUserId, testEventId);

            int count = sentry.GetContext().EventLinks.Count();

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
