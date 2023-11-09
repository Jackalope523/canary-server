using Microsoft.EntityFrameworkCore;
using Core.Boundaries;
using Xunit.Abstractions;
using Repository.Repository;

namespace Repository.Tests
{
    public class EventTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static EventStore store = new EventStore(sentry);
        private static UserFactory userFactory = new UserFactory();
        private static EventFactory eventFactory = new EventFactory();

        private readonly ITestOutputHelper _testOutputHelper;

        private User testUser;
        private Event testEvent;

        public EventTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            testUser = userFactory.Create();
            sentry.ExecuteWrite(ctx => ctx.Users.Add(testUser));

            testEvent = eventFactory.Create();
            testEvent.HostId = testUser.Id;
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
            Assert.Equal(testEvent.Name, found.Name);
            Assert.Equal(testEvent.Description, found.Description);
            Assert.Equal(testEvent.StartTime, found.StartTime);
            Assert.Equal(testEvent.Location.Y, found.Latitude);
            Assert.Equal(testEvent.Location.X, found.Longitude);
            Assert.Equal(testEvent.GroupMinimum, found.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, found.GroupMaximum);
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
            Assert.Equal(testEvent.Location.Y, found.First().Latitude);
            Assert.Equal(testEvent.Location.X, found.First().Longitude);
           
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
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.NotEqual(testEvent.Description, updated.Description);
            Assert.Equal(newDescription, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
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
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
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
            Assert.Equal(testUser.Id, ended.HostId);
            Assert.Equal(testEvent.Name, ended.Name);
            Assert.Equal(testEvent.Description, ended.Description);
            Assert.Equal(testEvent.StartTime, ended.StartTime);
            Assert.Equal(testEvent.Location.Y, ended.Location.Y);
            Assert.Equal(testEvent.Location.X, ended.Location.X);
            Assert.Equal(testEvent.GroupMinimum, ended.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, ended.GroupMaximum);
            Assert.Equal(endTime, ended.EndTime);
        }      
    }
}
