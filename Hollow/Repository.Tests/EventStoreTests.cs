using Microsoft.EntityFrameworkCore;
using Core.Boundaries;
using Xunit.Abstractions;
using NetTopologySuite.Geometries;
using System.Security.Cryptography.X509Certificates;

namespace Repository.Tests
{
    public class EventStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static EventStore store = new EventStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User testUser;
        private Event testEvent;

        public EventStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            testUser = new UserFactory().Create();
            sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(testUser));

            testEvent = new EventFactory().Create(testUser);
        }
        public void Dispose()
        {
            sentry.ExecuteWriteAsync(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWriteAsync(ctx => ctx.Events.ExecuteDelete());
        }

        
        [Fact]
        public async Task CreateEventAsync_SUCCESS()
        {
            await store.CreateEventAsync(
                testEvent.HostId, 
                testEvent.Name, 
                testEvent.Description, 
                testEvent.StartTime, 
                testEvent.Location.Y, 
                testEvent.Location.X, 
                testEvent.GroupMinimum, 
                testEvent.GroupMaximum,
                new Character(
                    testEvent.Extroversion,
                    testEvent.Athleticisme,
                    testEvent.Chaos,
                    testEvent.Competitiveness,
                    testEvent.Industriousness,
                    testEvent.NightOwl,
                    testEvent.Openness
                    ));

            Event created = sentry.ExecuteRead(ctx => ctx.Events.First());

            Assert.NotNull(created);
            Assert.Equal(testEvent.HostId, created.HostId);
            Assert.Equal(testEvent.Name, created.Name);
            Assert.Equal(testEvent.Description, created.Description);
            Assert.Equal(testEvent.StartTime, created.StartTime);
            Assert.Equal(testEvent.Location.Y, created.Location.Y);
            Assert.Equal(testEvent.Location.X, created.Location.X);
            Assert.Equal(testEvent.GroupMinimum, created.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, created.GroupMaximum);
            Assert.Equal(testEvent.IsOpen, created.IsOpen);
        }       
        [Fact]
        public async Task FindEventAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Events.Add(testEvent));

            EventShard found = await store.FindEventAsync(testEvent.Id);

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
        public async Task FindEventsAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Events.Add(testEvent));

            Point location = testEvent.Location;
            List<EventThinSlice> found = await store.FindEventsAsync(location.Y, location.X, 10);

            Assert.Single(found);
            Assert.Equal(testEvent.Id, found.First().Id);
            Assert.Equal(testUser.Id, found.First().Host.Id);
            Assert.Equal(testUser.Name, found.First().Host.Name);
            Assert.Equal(testEvent.Location.Y, found.First().Latitude);
            Assert.Equal(testEvent.Location.X, found.First().Longitude);
        }
        [Fact]
        public async Task UpdateEventAsync_Description()
        {
            string newDescription = "The Second of few.";

            await sentry.ExecuteWriteAsync(ctx =>ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Description", newDescription));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = await sentry.ExecuteReadAsync(ctx => ctx.Events.FirstAsync());

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
        public async Task UpdateEventAsync_Status()
        {
            bool newStatus = true;

            await sentry.ExecuteWriteAsync(ctx => ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("IsOpen", newStatus));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = await sentry.ExecuteReadAsync(ctx => ctx.Events.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.True(updated.IsOpen);
        }
        [Fact]
        public async Task EndEventAsync_SUCCESS()
        {
            DateTimeOffset endTime = DateTimeOffset.UtcNow;

            await sentry.ExecuteWriteAsync(ctx => ctx.Events.Add(testEvent));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("EndTime", endTime));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event ended = await sentry.ExecuteReadAsync(ctx => ctx.Events.FirstAsync());

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
        [Fact]
        public async Task FindCurrentEventForUserAsync_SUCCESS() 
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventLink.EventLinkType.Present);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.AddAsync(link));

            EventShard @event = await store.FindCurrentEventForUserAsync(testUser.Id);

            Assert.NotNull(@event);
            Assert.Equal(testEvent.HostId, @event.Host.Id);
            Assert.Equal(testEvent.Name, @event.Name);
            Assert.Equal(testEvent.Description, @event.Description);
            Assert.Equal(testEvent.StartTime, @event.StartTime);
            Assert.Equal(testEvent.Location.Y, @event.Latitude);
            Assert.Equal(testEvent.Location.X, @event.Longitude);
            Assert.Equal(testEvent.GroupMinimum, @event.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, @event.GroupMaximum);
            Assert.Equal(testEvent.IsOpen, @event.IsOpen);
        }
        [Fact]
        public async Task FindUpcomingEventsForUserAsync_SUCCESS() 
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventLink.EventLinkType.Attend);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.AddAsync(link));

            EventShard @event = (await store.FindUpcomingEventsForUserAsync(testUser.Id)).First();

            Assert.NotNull(@event);
            Assert.Equal(testEvent.HostId, @event.Host.Id);
            Assert.Equal(testEvent.Name, @event.Name);
            Assert.Equal(testEvent.Description, @event.Description);
            Assert.Equal(testEvent.StartTime, @event.StartTime);
            Assert.Equal(testEvent.Location.Y, @event.Latitude);
            Assert.Equal(testEvent.Location.X, @event.Longitude);
            Assert.Equal(testEvent.GroupMinimum, @event.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, @event.GroupMaximum);
            Assert.Equal(testEvent.IsOpen, @event.IsOpen);
        }
        [Fact]
        public async Task FindPastEventsForUserAsync_SUCCESS() 
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventLink.EventLinkType.Left);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.AddAsync(link));

            EventShard @event = (await store.FindPastEventsForUserAsync(testUser.Id)).First();

            Assert.NotNull(@event);
            Assert.Equal(testEvent.HostId, @event.Host.Id);
            Assert.Equal(testEvent.Name, @event.Name);
            Assert.Equal(testEvent.Description, @event.Description);
            Assert.Equal(testEvent.StartTime, @event.StartTime);
            Assert.Equal(testEvent.Location.Y, @event.Latitude);
            Assert.Equal(testEvent.Location.X, @event.Longitude);
            Assert.Equal(testEvent.GroupMinimum, @event.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, @event.GroupMaximum);
            Assert.Equal(testEvent.IsOpen, @event.IsOpen);
        }   
        [Fact]
        public async Task AddUserToEventAsync_SUCCESS() 
        {
            await store.AddUserToEventAsync(testUser.Id, testEvent.Id);

            EventLink link = await sentry.ExecuteReadAsync(ctx => ctx.EventLinks.SingleAsync());

            Assert.Equal(testUser.Id, link.SelfId);
            Assert.Equal(testEvent.Id, link.OtherId);
            Assert.Equal(EventLink.EventLinkType.Attend, link.Type);
        }
        [Fact]
        public async Task RemoveUserFromEventAsync_SUCCESS() 
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventLink.EventLinkType.Attend);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.AddAsync(link));

            await store.RemoveUserFromEventAsync(testUser.Id, testEvent.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.EventLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetGuestHistoryAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventLink.EventLinkType.Left);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.AddAsync(link));

            (DateTimeOffset, DateTimeOffset?, UserSilhouette) guest = (await store.GetGuestHistoryAsync(testEvent.Id)).First();

            Assert.NotNull(guest.Item2);
            Assert.NotNull(guest.Item3);
            Assert.Equal(DateTimeOffset.MinValue, guest.Item1);
            Assert.Equal(testUser.Id, guest.Item3.Id);
            Assert.Equal(testUser.Name, guest.Item3.Name);
        }
        [Fact]
        public async Task GetGuestListAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventLink.EventLinkType.Attend);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventLinks.AddAsync(link));

            UserSilhouette user = (await store.GetGuestListAsync(testEvent.Id)).First();

            Assert.NotNull(user);
            Assert.Equal(testUser.Id, user.Id);
            Assert.Equal(testUser.Name, user.Name);
        }
    }
}
