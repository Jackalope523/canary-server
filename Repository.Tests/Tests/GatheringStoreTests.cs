using Microsoft.EntityFrameworkCore;
using Core.Boundaries;
using Xunit.Abstractions;
using NetTopologySuite.Geometries;

using Microsoft.Extensions.Logging;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class GatheringStoreTests : IDisposable
    {
        private static EFCoreSentry sentry = new(Harbor.Flag.Production);
        private static EFCoreGatheringStore store = new(Harbor.Flag.Production);

        private readonly ITestOutputHelper _testOutputHelper;

        private User testUser;
        private Gathering testGathering;

        public GatheringStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            testUser = new UserFactory().Create();
            sentry.ExecuteWrite(ctx => ctx.Users.Add(testUser));

            testGathering = new GatheringFactory().Create(testUser);
            sentry.ExecuteWrite(ctx => ctx.Gatherings.Add(testGathering));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Gatherings.ExecuteDelete());
        }


        [Fact]
        public async Task CreateGatheringAsync_SUCCESS()
        {
            CoreGathering createdShard = await store.CreateGatheringAsync(
                testGathering.HostId,
                testGathering.Name,
                testGathering.Description,
                testGathering.StartTime,
                testGathering.Location.Y,
                testGathering.Location.X,
                testGathering.FriendlyLocation,
                testGathering.GroupMinimum,
                testGathering.GroupMaximum,
                new CharacterShard(
                    testGathering.Age,
                    testGathering.Extroversion,
                    testGathering.Athleticisme,
                    testGathering.Chaos,
                    testGathering.Competitiveness,
                    testGathering.Industriousness,
                    testGathering.NightOwl,
                    testGathering.Openness),
                testGathering.Radius,
                testGathering.IsDynamic,
                testGathering.DegreeOfPrivacy
                );

            Gathering created = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == createdShard.Id).Single());

            Assert.NotNull(created);
            Assert.Equal(testGathering.HostId, created.HostId);
            Assert.Equal(testGathering.Name, created.Name);
            Assert.Equal(testGathering.Description, created.Description);
            Assert.Equal(testGathering.StartTime, created.StartTime);
            Assert.Equal(testGathering.Location.Y, created.Location.Y);
            Assert.Equal(testGathering.Location.X, created.Location.X);
            Assert.Equal(testGathering.GroupMinimum, created.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, created.GroupMaximum);
            Assert.Equal(testGathering.State, created.State);
            Assert.Equal(testGathering.State, created.State);
            Assert.Equal(testGathering.State, created.State);
            Assert.Equal(testGathering.State, created.State);
        }
        [Fact]
        public async Task FindGatheringAsync_SUCCESS()
        {
            CoreGathering found = await store.FindGatheringAsync(testGathering.Id);

            Assert.NotNull(found);
            Assert.Equal(testGathering.Id, found.Id);
            Assert.Equal(testUser.Id, found.Host.Id);
            Assert.Equal(testUser.Name, found.Host.Name);
            Assert.Equal(testGathering.Name, found.Title);
            Assert.Equal(testGathering.Description, found.Description);
            Assert.Equal(testGathering.StartTime, found.StartTime);
            Assert.Equal(testGathering.Location.Y, found.Latitude);
            Assert.Equal(testGathering.Location.X, found.Longitude);
            Assert.Equal(testGathering.GroupMinimum, found.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, found.GroupMaximum);
        }
        [Fact]
        public async Task UpdateGatheringAsync_Description()
        {
            string newDescription = "The Second of few.";

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.Description), newDescription));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.NotEqual(testGathering.Description, updated.Description);
            Assert.Equal(newDescription, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_Status()
        {
            GatheringState newState = GatheringState.Ended;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.State), newState));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.NotEqual(testGathering.State, updated.State);
            Assert.Equal(newState, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_StartTime()
        {
            DateTimeOffset newTime = DateTimeOffset.UtcNow;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.StartTime), newTime));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.NotEqual(testGathering.StartTime, updated.StartTime);
            Assert.Equal(newTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_Latitude()
        {
            double newLatitude = 34.052;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Location", (newLatitude, testGathering.Location.X)));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.NotEqual(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(newLatitude, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_Longitude()
        {
            double newLongitude = -118.243;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Location", (testGathering.Location.Y, newLongitude)));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.NotEqual(testGathering.Location.X, updated.Location.X);
            Assert.Equal(newLongitude, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_Radius()
        {
            double newRadius = 27.056;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.Radius), newRadius));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.NotEqual(testGathering.Radius, updated.Radius);
            Assert.Equal(newRadius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_IsDynamic()
        {
            bool newType = true;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.IsDynamic), newType));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.NotEqual(testGathering.IsDynamic, updated.IsDynamic);
            Assert.Equal(newType, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_GroupMinimum()
        {
            int newMinimum = 6;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.GroupMinimum), newMinimum));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.NotEqual(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.Equal(newMinimum, updated.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task UpdateGatheringAsync_GroupMaximum()
        {
            int newMaximum = 6;

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add((nameof(CoreGathering.GroupMaximum), newMaximum));

            await store.UpdateGatheringAsync(testGathering.Id, updates);

            Gathering updated = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.NotNull(updated);
            Assert.Equal(testUser.Id, updated.HostId);
            Assert.Equal(testGathering.Name, updated.Name);
            Assert.Equal(testGathering.Description, updated.Description);
            Assert.Equal(testGathering.StartTime, updated.StartTime);
            Assert.Equal(testGathering.Location.Y, updated.Location.Y);
            Assert.Equal(testGathering.Location.X, updated.Location.X);
            Assert.Equal(testGathering.GroupMinimum, updated.GroupMinimum);
            Assert.NotEqual(testGathering.GroupMaximum, updated.GroupMaximum);
            Assert.Equal(newMaximum, updated.GroupMaximum);
            Assert.Equal(testGathering.State, updated.State);
            Assert.Equal(testGathering.Radius, updated.Radius);
            Assert.Equal(testGathering.IsDynamic, updated.IsDynamic);
        }
        [Fact]
        public async Task FindCurrentGatheringForUserAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Arrived);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteUpdate(setter => setter.SetProperty(u => u.CurrentGathering, testGathering.Id)));

            CoreGathering gathering = await store.FindCurrentGatheringForUserAsync(testUser.Id);

            Assert.NotNull(gathering);
            Assert.Equal(testGathering.HostId, gathering.Host.Id);
            Assert.Equal(testUser.Name, gathering.Host.Name);
            Assert.Equal(testGathering.Name, gathering.Title);
            Assert.Equal(testGathering.Description, gathering.Description);
            Assert.Equal(testGathering.StartTime, gathering.StartTime);
            Assert.Equal(testGathering.Location.Y, gathering.Latitude);
            Assert.Equal(testGathering.Location.X, gathering.Longitude);
            Assert.Equal(testGathering.GroupMinimum, gathering.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, gathering.GroupMaximum);
            Assert.Equal(testGathering.State, gathering.State);
        }
        [Fact]
        public async Task FindUpcomingGatheringsForUserAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));

            CoreGathering gathering = (await store.FindUpcomingGatheringsForUserAsync(testUser.Id)).First();

            Assert.NotNull(gathering);
            Assert.Equal(testGathering.HostId, gathering.Host.Id);
            Assert.Equal(testGathering.Name, gathering.Title);
            Assert.Equal(testGathering.Description, gathering.Description);
            Assert.Equal(testGathering.StartTime, gathering.StartTime);
            Assert.Equal(testGathering.Location.Y, gathering.Latitude);
            Assert.Equal(testGathering.Location.X, gathering.Longitude);
            Assert.Equal(testGathering.GroupMinimum, gathering.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, gathering.GroupMaximum);
            Assert.Equal(testGathering.State, gathering.State);
        }
        [Fact]
        public async Task FindPastGatheringsForUserAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Left);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));
            sentry.ExecuteWrite(ctx => 
                   ctx.Gatherings.
                   Where(e => e.Id == testGathering.Id).
                   ExecuteUpdate(setter => setter.SetProperty(e => e.State, GatheringState.Ended)));

            CoreGathering gathering = (await store.FindPastGatheringsForUserAsync(testUser.Id)).First();

            Assert.NotNull(gathering);
            Assert.Equal(testGathering.HostId, gathering.Host.Id);
            Assert.Equal(testUser.Name, gathering.Host.Name);
            Assert.Equal(testGathering.Name, gathering.Title);
            Assert.Equal(testGathering.Description, gathering.Description);
            Assert.Equal(testGathering.StartTime, gathering.StartTime);
            Assert.Equal(testGathering.Location.Y, gathering.Latitude);
            Assert.Equal(testGathering.Location.X, gathering.Longitude);
            Assert.Equal(testGathering.GroupMinimum, gathering.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, gathering.GroupMaximum);
            Assert.Equal(GatheringState.Ended, gathering.State);
        }       
        [Fact]
        public async Task RemoveUserAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));

            await store.DeleteUserStateAsync(testUser.Id, testGathering.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetGuestHistoryAsync_Left()
        {
            GatheringLinkFactory factory = new GatheringLinkFactory();
            GatheringLink arrivalLink = factory.Create(testUser, testGathering, GatheringBond.Arrived, DateTimeOffset.MinValue);
            GatheringLink departureLink = factory.Create(testUser, testGathering, GatheringBond.Left, DateTimeOffset.MaxValue);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(arrivalLink));
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(departureLink));

            (DateTimeOffset, DateTimeOffset?, UserShard) guest = (await store.GetGuestHistoryAsync(testGathering.Id)).First();

            Assert.NotNull(guest.Item3);
            Assert.Equal(DateTimeOffset.MinValue, guest.Item1);
            Assert.Equal(DateTimeOffset.MaxValue, guest.Item2);
            Assert.Equal(testUser.Id, guest.Item3.Id);
            Assert.Equal(testUser.Name, guest.Item3.Name);
        }
        [Fact]
        public async Task GetGuestHistoryAsync_NotLeft()
        {
            GatheringLink arrivalLink = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Arrived, DateTimeOffset.MinValue);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(arrivalLink));

            (DateTimeOffset, DateTimeOffset?, UserShard) guest = (await store.GetGuestHistoryAsync(testGathering.Id)).First();

            Assert.NotNull(guest.Item3);
            Assert.Equal(DateTimeOffset.MinValue, guest.Item1);
            Assert.Null(guest.Item2);
            Assert.Equal(testUser.Id, guest.Item3.Id);
            Assert.Equal(testUser.Name, guest.Item3.Name);
        }
        [Fact]
        public async Task GetGuestListAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));

            UserShard user = (await store.GetGuestListAsync(testGathering.Id)).Single();

            Assert.NotNull(user);
            Assert.Equal(testUser.Id, user.Id);
            Assert.Equal(testUser.Name, user.Name);
        }       
        [Fact]
        public async Task EndGatheringAsync_SUCCESS()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Arrived, DateTimeOffset.MinValue);

            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));
            sentry.ExecuteWrite(ctx => 
                ctx.Users.
                Where(u => u.Id == testUser.Id).
                ExecuteUpdate(setter => setter.SetProperty(u => u.CurrentGathering, testGathering.Id)));

            await store.TerminateGatheringAsync(testGathering.Id, time);

            List<GatheringLink> links = await sentry.ExecuteReadAsync(ctx => 
                ctx.GatheringLinks.
                Where(l => l.UserId == testUser.Id).
                ToListAsync());

            links.Sort((x, y) => DateTimeOffset.Compare(x.Time, y.Time));

            Gathering ended = sentry.ExecuteRead(ctx => ctx.Gatherings.Where(e => e.Id == testGathering.Id).Single());

            Assert.Equal(DateTimeOffset.MinValue, links.First().Time);
            Assert.Equal(testUser.Id, links.First().UserId);
            Assert.Equal(testGathering.Id, links.First().GatheringId);
            Assert.Equal(GatheringBond.Arrived, links.First().Type);

            Assert.Equal(testUser.Id, links.Last().UserId);
            Assert.Equal(testGathering.Id, links.Last().GatheringId);
            Assert.Equal(GatheringBond.Left, links.Last().Type);

            Assert.NotNull(ended);
            Assert.Equal(testUser.Id, ended.HostId);
            Assert.Equal(testGathering.Name, ended.Name);
            Assert.Equal(testGathering.Description, ended.Description);
            Assert.Equal(testGathering.StartTime, ended.StartTime);
            Assert.Equal(testGathering.Location.Y, ended.Location.Y);
            Assert.Equal(testGathering.Location.X, ended.Location.X);
            Assert.Equal(testGathering.GroupMinimum, ended.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, ended.GroupMaximum);
            Assert.Equal(GatheringState.Ended, ended.State);
            Assert.NotNull(ended.EndTime);
        }
        [Fact]
        public async Task FindGatheringsByUserAsync_SUCCESS()
        {
            CoreGathering gathering  = (await store.FindGatheringsByUserAsync(testUser.Id)).Single();

            Assert.NotNull(gathering);
            Assert.Equal(testGathering.HostId, gathering.Host.Id);
            Assert.Equal(testGathering.Name, gathering.Title);
            Assert.Equal(testGathering.Description, gathering.Description);
            Assert.Equal(testGathering.StartTime, gathering.StartTime);
            Assert.Equal(testGathering.Location.Y, gathering.Latitude);
            Assert.Equal(testGathering.Location.X, gathering.Longitude);
            Assert.Equal(testGathering.GroupMinimum, gathering.GroupMinimum);
            Assert.Equal(testGathering.GroupMaximum, gathering.GroupMaximum);
            Assert.Equal(testGathering.State, gathering.State);
        }
        [Fact]
        public async Task GetUserStateAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Guest);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));

            GatheringBond? state = await store.GetUserStateAsync(testUser.Id, testGathering.Id);

            Assert.Equal(GatheringBond.Guest, state);
        }
        [Fact]
        public async Task SetUserStateAsync_SUCCESS()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;
            await store.SetUserStateAsync(testUser.Id, testGathering.Id, GatheringBond.Guest, time);

            GatheringLink link = await sentry.ExecuteReadAsync(ctx => ctx.GatheringLinks.SingleAsync());

            Assert.NotNull(link);
            Assert.Equal(testUser.Id, link.UserId);
            Assert.Equal(testGathering.Id, link.GatheringId);
            Assert.Equal(GatheringBond.Guest, link.Type);
        }
        [Fact]
        public async Task GetAllUsersAsync_SUCCESS()
        {
            GatheringLink link = new GatheringLinkFactory().Create(testUser, testGathering, GatheringBond.Watching, DateTimeOffset.MinValue);
            sentry.ExecuteWrite(ctx => ctx.GatheringLinks.Add(link));

            (UserShard User, GatheringBond State) = (await store.GetAllUsersAsync(testGathering.Id)).Single();

            Assert.Equal(testUser.Id, User.Id);
            Assert.Equal(testUser.Name, User.Name);
            Assert.Equal(GatheringBond.Watching, State);
        }
        [Fact]
        public async Task DeleteGatheringAsync_SUCCESS()
        {
            await store.DeleteGatheringAsync(testGathering.Id);

            int count = sentry.ExecuteRead(ctx => ctx.Gatherings.Count());

            Assert.Equal(0, count);
        }      
    }
}
