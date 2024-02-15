using Microsoft.EntityFrameworkCore;
using Core.Boundaries;
using Xunit.Abstractions;
using NetTopologySuite.Geometries;
using Shared;

namespace Repository.Tests
{
    [Collection("Database Collection")]
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
            sentry.ExecuteWrite(ctx => ctx.Users.Add(testUser));

            testEvent = new EventFactory().Create(testUser);
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.EventLinks.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Events.ExecuteDelete());
        }


        [Fact]
        public async Task CreateEventAsync_SUCCESS()
        {
            EventShard createdShard = await store.CreateEventAsync(
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
                    testEvent.Openness),
                testEvent.Radius,
                testEvent.IsDynamic
                );

            Event created = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == createdShard.Id).Single());

            Assert.NotNull(created);
            Assert.Equal(testEvent.HostId, created.HostId);
            Assert.Equal(testEvent.Name, created.Name);
            Assert.Equal(testEvent.Description, created.Description);
            Assert.Equal(testEvent.StartTime, created.StartTime);
            Assert.Equal(testEvent.Location.Y, created.Location.Y);
            Assert.Equal(testEvent.Location.X, created.Location.X);
            Assert.Equal(testEvent.GroupMinimum, created.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, created.GroupMaximum);
            Assert.Equal(testEvent.State, created.State);
        }
        [Fact]
        public async Task FindEventAsync_SUCCESS()
        {
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

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.Description), newDescription));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

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
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_Status()
        {
            EventState newState = EventState.Ended;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.State), newState));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.NotEqual(testEvent.State, updated.State);
            Assert.Equal(newState, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_StartTime()
        {
            DateTimeOffset newTime = DateTimeOffset.UtcNow;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.StartTime), newTime));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.NotEqual(testEvent.StartTime, updated.StartTime);
            Assert.Equal(newTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_Latitude()
        {
            double newLatitude = 34.052;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.Latitude), newLatitude));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.NotEqual(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(newLatitude, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_Longitude()
        {
            double newLongitude = -118.243;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.Longitude), newLongitude));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.NotEqual(testEvent.Location.X, updated.Location.X);
            Assert.Equal(newLongitude, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_Radius()
        {
            double newRadius = 27.056;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.Radius), newRadius));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.NotEqual(testEvent.Radius, updated.Radius);
            Assert.Equal(newRadius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_IsDynamic()
        {
            bool newType = true;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.IsDynamic), newType));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.NotEqual(testEvent.IsDynamic, updated.IsDynamic);
            Assert.Equal(newType, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_GroupMinimum()
        {
            int newMinimum = 6;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.GroupMinimum), newMinimum));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.NotEqual(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(newMinimum, updated.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateEventAsync_GroupMaximum()
        {
            int newMaximum = 6;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(EventShard.GroupMaximum), newMaximum));

            await store.UpdateEventAsync(testEvent.Id, updates);

            Event updated = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testEvent.Name, updated.Name);
            Assert.Equal(testEvent.Description, updated.Description);
            Assert.Equal(testEvent.StartTime, updated.StartTime);
            Assert.Equal(testEvent.Location.Y, updated.Location.Y);
            Assert.Equal(testEvent.Location.X, updated.Location.X);
            Assert.Equal(testEvent.GroupMinimum, updated.GroupMinimum);
            Assert.NotEqual(testEvent.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(newMaximum, updated.GroupMaximum);
            Assert.Equal(testEvent.State, updated.State);
            Assert.Equal(testEvent.Radius, updated.Radius);
            Assert.Equal(testEvent.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task FindCurrentEventForUserAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Arrived);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteUpdate(setter => setter.SetProperty(u => u.CurrentEvent, testEvent.Id)));

            EventShard @event = await store.FindCurrentEventForUserAsync(testUser.Id);

            Assert.NotNull(@event);
            Assert.Equal(testEvent.HostId, @event.Host.Id);
            Assert.Equal(testUser.Name, @event.Host.Name);
            Assert.Equal(testEvent.Name, @event.Name);
            Assert.Equal(testEvent.Description, @event.Description);
            Assert.Equal(testEvent.StartTime, @event.StartTime);
            Assert.Equal(testEvent.Location.Y, @event.Latitude);
            Assert.Equal(testEvent.Location.X, @event.Longitude);
            Assert.Equal(testEvent.GroupMinimum, @event.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, @event.GroupMaximum);
            Assert.Equal(testEvent.State, @event.State);
        }
        [Fact]
        public async Task FindUpcomingEventsForUserAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));

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
            Assert.Equal(testEvent.State, @event.State);
        }
        [Fact]
        public async Task FindPastEventsForUserAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Left);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));
            sentry.ExecuteWrite(ctx => 
                   ctx.Events.
                   Where(e => e.Id == testEvent.Id).
                   ExecuteUpdate(setter => setter.SetProperty(e => e.State, EventState.Ended)));

            EventShard @event = (await store.FindPastEventsForUserAsync(testUser.Id)).First();

            Assert.NotNull(@event);
            Assert.Equal(testEvent.HostId, @event.Host.Id);
            Assert.Equal(testUser.Name, @event.Host.Name);
            Assert.Equal(testEvent.Name, @event.Name);
            Assert.Equal(testEvent.Description, @event.Description);
            Assert.Equal(testEvent.StartTime, @event.StartTime);
            Assert.Equal(testEvent.Location.Y, @event.Latitude);
            Assert.Equal(testEvent.Location.X, @event.Longitude);
            Assert.Equal(testEvent.GroupMinimum, @event.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, @event.GroupMaximum);
            Assert.Equal(EventState.Ended, @event.State);
        }       
        [Fact]
        public async Task RemoveUserAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));

            await store.RemoveUserAsync(testUser.Id, testEvent.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.EventLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetGuestHistoryAsync_Left()
        {
            EventLinkFactory factory = new EventLinkFactory();
            EventLink arrivalLink = factory.Create(testUser, testEvent, EventBond.Arrived, DateTimeOffset.MinValue);
            EventLink departureLink = factory.Create(testUser, testEvent, EventBond.Left, DateTimeOffset.MaxValue);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(arrivalLink));
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(departureLink));

            (DateTimeOffset, DateTimeOffset?, UserSilhouette) guest = (await store.GetGuestHistoryAsync(testEvent.Id)).First();

            Assert.NotNull(guest.Item3);
            Assert.Equal(DateTimeOffset.MinValue, guest.Item1);
            Assert.Equal(DateTimeOffset.MaxValue, guest.Item2);
            Assert.Equal(testUser.Id, guest.Item3.Id);
            Assert.Equal(testUser.Name, guest.Item3.Name);
        }
        [Fact]
        public async Task GetGuestHistoryAsync_NotLeft()
        {
            EventLink arrivalLink = new EventLinkFactory().Create(testUser, testEvent, EventBond.Arrived, DateTimeOffset.MinValue);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(arrivalLink));

            (DateTimeOffset, DateTimeOffset?, UserSilhouette) guest = (await store.GetGuestHistoryAsync(testEvent.Id)).First();

            Assert.NotNull(guest.Item3);
            Assert.Equal(DateTimeOffset.MinValue, guest.Item1);
            Assert.Null(guest.Item2);
            Assert.Equal(testUser.Id, guest.Item3.Id);
            Assert.Equal(testUser.Name, guest.Item3.Name);
        }
        [Fact]
        public async Task GetGuestListAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));

            UserSilhouette user = (await store.GetGuestListAsync(testEvent.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(testUser.Id, user.Id);
            Assert.Equal(testUser.Name, user.Name);
        }       
        [Fact]
        public async Task EndEventAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Arrived, DateTimeOffset.MinValue);

            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));
            sentry.ExecuteWrite(ctx => 
                ctx.Users.
                Where(u => u.Id == testUser.Id).
                ExecuteUpdate(setter => setter.SetProperty(u => u.CurrentEvent, testEvent.Id)));

            await store.EndEventAsync(testEvent.Id);

            List<EventLink> links = await sentry.ExecuteReadAsync(ctx => 
                ctx.EventLinks.
                Where(l => l.UserId == testUser.Id).
                ToListAsync());

            links.Sort((x, y) => DateTimeOffset.Compare(x.Time, y.Time));

            Event ended = sentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == testEvent.Id).Single());

            Assert.Equal(DateTimeOffset.MinValue, links.First().Time);
            Assert.Equal(testUser.Id, links.First().UserId);
            Assert.Equal(testEvent.Id, links.First().EventId);
            Assert.Equal(EventBond.Arrived, links.First().Type);

            Assert.Equal(testUser.Id, links.Last().UserId);
            Assert.Equal(testEvent.Id, links.Last().EventId);
            Assert.Equal(EventBond.Left, links.Last().Type);

            Assert.NotNull(ended);
            Assert.Equal(testUser.Id, ended.HostId);
            Assert.Equal(testEvent.Name, ended.Name);
            Assert.Equal(testEvent.Description, ended.Description);
            Assert.Equal(testEvent.StartTime, ended.StartTime);
            Assert.Equal(testEvent.Location.Y, ended.Location.Y);
            Assert.Equal(testEvent.Location.X, ended.Location.X);
            Assert.Equal(testEvent.GroupMinimum, ended.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, ended.GroupMaximum);
            Assert.NotNull(ended.EndTime);
        }
        [Fact]
        public async Task FindEventsByUserAsync_SUCCESS()
        {
            EventShard @event  = (await store.FindEventsByUserAsync(testUser.Id)).Single();

            Assert.NotNull(@event);
            Assert.Equal(testEvent.HostId, @event.Host.Id);
            Assert.Equal(testEvent.Name, @event.Name);
            Assert.Equal(testEvent.Description, @event.Description);
            Assert.Equal(testEvent.StartTime, @event.StartTime);
            Assert.Equal(testEvent.Location.Y, @event.Latitude);
            Assert.Equal(testEvent.Location.X, @event.Longitude);
            Assert.Equal(testEvent.GroupMinimum, @event.GroupMinimum);
            Assert.Equal(testEvent.GroupMaximum, @event.GroupMaximum);
            Assert.Equal(testEvent.State, @event.State);
        }
        [Fact]
        public async Task GetUserStateAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));

            EventBond? state = await store.GetUserStateAsync(testUser.Id, testEvent.Id);

            Assert.Equal(EventBond.Guest, state);
        }
        [Fact]
        public async Task SetUserStateAsync_SUCCESS()
        {
            await store.SetUserStateAsync(testUser.Id, testEvent.Id, EventBond.Guest);

            EventLink link = await sentry.ExecuteReadAsync(ctx => ctx.EventLinks.SingleAsync());

            Assert.NotNull(link);
            Assert.Equal(testUser.Id, link.UserId);
            Assert.Equal(testEvent.Id, link.EventId);
            Assert.Equal(EventBond.Guest, link.Type);
        }
        [Fact]
        public async Task GetAllUsersAsync_SUCCESS()
        {
            EventLink link = new EventLinkFactory().Create(testUser, testEvent, EventBond.Watching, DateTimeOffset.MinValue);
            sentry.ExecuteWrite(ctx => ctx.EventLinks.Add(link));

            (UserSilhouette User, EventBond State) = (await store.GetAllUsersAsync(testEvent.Id)).Single();

            Assert.Equal(testUser.Id, User.Id);
            Assert.Equal(testUser.Name, User.Name);
            Assert.Equal(EventBond.Watching, State);
        }
        [Fact]
        public async Task DeleteEventAsync_SUCCESS()
        {
            await store.DeleteEventAsync(testEvent.Id);

            int count = sentry.ExecuteRead(ctx => ctx.Events.Count());

            Assert.Equal(0, count);
        }
    }
}
