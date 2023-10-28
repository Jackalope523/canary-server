using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Core.Boundaries;
using Xunit.Abstractions;


namespace Repository.Tests
{
    public class EventTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static EventStore store = new EventStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private Guid testHostId;
        private string testUserPhoneNumber = "000-000-0000";
        private string testUserEmail = "email_0@test.com";
        private string testUserNormalizedEmail = "email_0@test.com";
        private string testUserName = "name";
        private string testUserSecurityStamp = "stamp";
        private DateTimeOffset subjectDateOfBirth = new DateTimeOffset(new DateTime(0));
        private User testUser;

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

        public EventTests(ITestOutputHelper testOutputHelper)
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
            
            sentry.ExecuteWrite(ctx => ctx.Users.Add(testUser));

            testEvent = new Event
            {
                HostId = testUser.Id,
                Name = testEventName,
                Description = testEventDescription,
                Type = testEventEventType,
                StartTime = testEventStartTime,
                Location = new Point(testEventLongitude, testEventLatitude),
                GroupMinimum = testEventGroupMinimum,
                GroupMaximum = testEventGroupMaximum,
                IsEventOpen= testIsEventOpen
            };
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Events.ExecuteDelete());
        }

        /*
        [Fact]
        public void CreateEvent_SUCCESS()
        {
            store.CreateEvent(testHostId, testEventName, testEventDescription, testEventEventType, testEventStartTime, testEventLatitude, testEventLongitude, testEventGroupMinimum, testEventGroupMaximum);

            Event created = sentry.GetContext().Events.First();

            Assert.NotNull(created);
            Assert.Equal(testHostId, created.HostId);
            Assert.Equal(testEventName, created.Name);
            Assert.Equal(testEventDescription, created.Description);
            Assert.Equal(testEventEventType, created.Type);
            Assert.Equal(testEventStartTime, created.StartTime);
            Assert.Equal(testEventLatitude, created.Location.Y);
            Assert.Equal(testEventLongitude, created.Location.X);
            Assert.Equal(testEventGroupMinimum, created.GroupMinimum);
            Assert.Equal(testEventGroupMaximum, created.GroupMaximum);
            Assert.Equal(testIsEventOpen, created.IsEventOpen);
        }
        */

        [Fact]
        public void FindEvent_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));

            EventShard found = store.FindEvent(testEvent.Id);

            Assert.NotNull(found);
            Assert.Equal(testEvent.Id, found.Id);
            Assert.Equal(testUser.Id, found.Host.Id);
            Assert.Equal(testUser.Name, found.Host.Name);
            Assert.Equal(testEventName, found.Name);
            Assert.Equal(testEventDescription, found.Description);
            Assert.Equal(testEventStartTime, found.StartTime);
            Assert.Equal(testEventLatitude, found.Latitude);
            Assert.Equal(testEventLongitude, found.Longitude);
            Assert.Equal(testEventGroupMinimum, found.GroupMinimum);
            Assert.Equal(testEventGroupMaximum, found.GroupMaximum);
        }
        [Fact]
        public void FindEvents_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));

            List<EventThinSlice> found = store.FindEvents(100, 100, 10);        

            Assert.Single(found);
            Assert.Equal(testEvent.Id, found.First().Id);
            Assert.Equal(testUser.Id, found.First().Host.Id);
            Assert.Equal(testUser.Name, found.First().Host.Name);
            Assert.Equal(testEventLatitude, found.First().Latitude);
            Assert.Equal(testEventLongitude, found.First().Longitude);
           
        }
        [Fact]
        public void UpdateDescription_SUCCESS()
        {
            string newDescription = "The Second of few.";

            sentry.ExecuteWrite(ctx =>ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Description", newDescription));

            store.UpdateEvent(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.First());

            Assert.NotNull(updated);
            Assert.Equal(testHostId, updated.HostId);
            Assert.Equal(testEventName, updated.Name);
            Assert.NotEqual(testEventDescription, updated.Description);
            Assert.Equal(newDescription, updated.Description);
            Assert.Equal(testEventEventType, updated.Type);
            Assert.Equal(testEventStartTime, updated.StartTime);
            Assert.Equal(testEventLatitude, updated.Location.Y);
            Assert.Equal(testEventLongitude, updated.Location.X);
            Assert.Equal(testEventGroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEventGroupMaximum, updated.GroupMaximum);
        }
        [Fact]
        public void UpdateType_SUCCESS()
        {
            string newType = "Mono";

            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Description", newType));

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.First());

            Assert.NotNull(updated);
            Assert.Equal(testHostId, updated.HostId);
            Assert.Equal(testEventName, updated.Name);
            Assert.Equal(testEventDescription, updated.Description);
            Assert.NotEqual(testEventEventType, updated.Type);
            Assert.Equal(newType, updated.Type);
            Assert.Equal(testEventStartTime, updated.StartTime);
            Assert.Equal(testEventLatitude, updated.Location.Y);
            Assert.Equal(testEventLongitude, updated.Location.X);
            Assert.Equal(testEventGroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEventGroupMaximum, updated.GroupMaximum);
        }
        [Fact]
        public void UpdateStatus_SUCCESS()
        {
            bool newStatus = true;

            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("IsOpen", newStatus));

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.First());

            Assert.NotNull(updated);
            Assert.Equal(testHostId, updated.HostId);
            Assert.Equal(testEventName, updated.Name);
            Assert.Equal(testEventDescription, updated.Description);
            Assert.Equal(testEventEventType, updated.Type);
            Assert.Equal(testEventStartTime, updated.StartTime);
            Assert.Equal(testEventLatitude, updated.Location.Y);
            Assert.Equal(testEventLongitude, updated.Location.X);
            Assert.Equal(testEventGroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEventGroupMaximum, updated.GroupMaximum);
            Assert.True(updated.IsEventOpen);
        }
        [Fact]
        public void EndEvent_SUCCESS()
        {
            DateTimeOffset endTime = DateTimeOffset.UtcNow;

            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("EndTime", endTime));

            Event ended = sentry.ExecuteRead(ctx => ctx.Events.First());

            Assert.NotNull(ended);
            Assert.Equal(testHostId, ended.HostId);
            Assert.Equal(testEventName, ended.Name);
            Assert.Equal(testEventDescription, ended.Description);
            Assert.Equal(testEventEventType, ended.Type);
            Assert.Equal(testEventStartTime, ended.StartTime);
            Assert.Equal(testEventLatitude, ended.Location.Y);
            Assert.Equal(testEventLongitude, ended.Location.X);
            Assert.Equal(testEventGroupMinimum, ended.GroupMinimum);
            Assert.Equal(testEventGroupMaximum, ended.GroupMaximum);
            Assert.Equal(endTime, ended.EndTime);
        }      
    }
}
