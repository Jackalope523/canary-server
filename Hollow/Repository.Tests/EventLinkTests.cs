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

        private User testUser;
        private Event testEvent;

        public EventLinkTests(ITestOutputHelper testOutputHelper) 
        {
            _testOutputHelper = testOutputHelper;

            testUser = new UserFactory().Create();
            sentry.ExecuteWrite(ctx => ctx.Users.Add(testUser));

            testEvent = new EventFactory().Create();
            testEvent.HostId = testUser.Id;
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));
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
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(new EventLink { SelfId = testUser.Id, EventId = testEvent.Id, Type = EventLink.EventLinkType.Attend }));

            List<UserSilhouette> guestList = store.GetGuestList(testEvent.Id);

            Assert.Single(guestList);
            Assert.Equal(testUser.Id, guestList.First().Id);
            Assert.Equal(testUser.Name, guestList.First().Name);
        }
        [Fact]
        public void AddUserToEvent_SUCCESS() 
        {
            store.AddUserToEvent(testUser.Id, testEvent.Id);

            EventLink link = sentry.ExecuteRead(ctx => ctx.EventLinks.First());

            Assert.NotNull(link);
            Assert.Equal(testUser.Id, link.SelfId);
            Assert.Equal(testEvent.Id, link.EventId);
            Assert.Equal(EventLink.EventLinkType.Attend, link.Type);
        }
        [Fact]
        public void RemoveUserFromEvent_SUCCESS() 
        {
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(new EventLink { SelfId = testUser.Id, EventId = testEvent.Id, Type = EventLink.EventLinkType.Attend }));

            store.RemoveUserFromEvent(testUser.Id, testEvent.Id);

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
