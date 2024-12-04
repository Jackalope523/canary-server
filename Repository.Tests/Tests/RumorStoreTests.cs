using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class RumorStoreTests : IDisposable
    {
        private static EFCoreSentry sentry = new(Harbor.Flag.Development);
        private static EFCoreRumorStore store = new(Harbor.Flag.Development);

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ReaperObserver _reaper;

        private readonly UserFactory _userFactory;
        private readonly RumoredGatheringFactory _rumoredGatheringFactory;
        private readonly RumorFactory _rumorFactory;
        private readonly InvestigationFactory _investigationFactory;
        private readonly RumorReportFactory _rumorReportFactory;

        private User user;
        private RumoredGathering rumoredGathering;
        private Rumor rumor;

        public RumorStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _reaper = new ReaperObserver();

            _userFactory = new UserFactory(_reaper);
            _rumoredGatheringFactory = new RumoredGatheringFactory(_reaper);
            _rumorFactory = new RumorFactory(_reaper);
            _investigationFactory = new InvestigationFactory(_reaper);
            _rumorReportFactory = new RumorReportFactory(_reaper);

            user = _userFactory.Create();
            rumoredGathering = _rumoredGatheringFactory.Create();

            sentry.ExecuteWrite(ctx => ctx.AddRange(user, rumoredGathering));

            rumor = _rumorFactory.Create(user, rumoredGathering);

            sentry.ExecuteWrite(ctx => ctx.Add(rumor));
        }
        public void Dispose()
        {
            _reaper.Reap(sentry);
        }


        [Fact]
        public async Task CreateRumorAsync_SUCCESS()
        {
            await store.CreateRumorAsync(rumoredGathering.Id, user.Id, Rumor.DefaultText, Rumor.DefaultTime);

            Rumor created = sentry.ExecuteRead(ctx => ctx.Rumors.ToList()).Last();
            _reaper.Notify(created);

            Assert.NotNull(created);
            Assert.Equal(user.Id, created.AuthorId);
            Assert.Equal(rumoredGathering.Id, created.RumoredGatheringId);
            Assert.Equal(Rumor.DefaultText, created.Text);
            Assert.Equal(Rumor.DefaultTime, created.Time);
            Assert.Equal(Rumor.DefaultSoftDeleted, created.SoftDeleted);
        }
        [Fact]
        public async Task CreateRumoredGatheringAsync_SUCCESS()
        {
            await store.CreateRumoredGatheringAsync(RumoredGathering.DefaultLocation.Y, RumoredGathering.DefaultLocation.X, RumoredGathering.DefaultFriendlyLocation);

            RumoredGathering created = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.ToList()).Last();
            _reaper.Notify(created);

            Assert.NotNull(created);
            Assert.Equal(RumoredGathering.DefaultLocation.X, created.Location.X);
            Assert.Equal(RumoredGathering.DefaultLocation.Y, created.Location.Y);
            Assert.Equal(RumoredGathering.DefaultFriendlyLocation, created.FriendlyLocation);
            Assert.Equal(RumoredGathering.DefaultConfidenceRating, created.ConfidenceRating);
            Assert.Equal(RumoredGathering.DefaultSoftDeleted, created.SoftDeleted);
        }
        [Fact]
        public async Task GetFounderAsync_SUCCESS()
        {
            UserShard founder = await store.GetFounderAsync(rumoredGathering.Id);

            Assert.NotNull(founder);
            Assert.Equal(user.Id, founder.Id);
            Assert.Equal(user.Name, founder.Name);
        }
        [Fact]
        public async Task GetRumorAsync_SUCCESS()
        {
            CoreRumor got = await store.GetRumorAsync(rumor.Id);

            Assert.NotNull(got);
            Assert.Equal(rumor.Id, got.Id);
            Assert.Equal(rumor.Text, got.Text);
            Assert.Equal(rumor.Time, got.Time);
        }
        [Fact]
        public async Task GetRumoredGatheringAsync_SUCCESS()
        {
            CoreRumoredGathering got = await store.GetRumoredGatheringAsync(rumoredGathering.Id);

            Assert.NotNull(got);
            Assert.Equal(rumoredGathering.Id, got.Id);
            Assert.Equal(rumoredGathering.FriendlyLocation, got.FriendlyLocation);
            Assert.Equal(rumoredGathering.ConfidenceRating, got.TrueConfidenceRating);
            Assert.Equal(rumoredGathering.Location.Y, got.Latitude);
            Assert.Equal(rumoredGathering.Location.X, got.Longitude);
        }
        [Fact]
        public async Task GetRumorsAboutAsync_SUCCESS()
        {
            CoreRumor got = (await store.GetRumorsAboutAsync(rumoredGathering.Id)).Single();

            Assert.NotNull(got);
            Assert.Equal(rumor.Id, got.Id);
            Assert.Equal(rumor.Text, got.Text);
            Assert.Equal(rumor.Time, got.Time);
        }
        [Fact]
        public async Task GetRumorsByAsync_SUCCESS()
        {
            CoreRumor got = (await store.GetRumorsByAsync(user.Id)).Single();

            Assert.NotNull(got);
            Assert.Equal(rumor.Id, got.Id);
            Assert.Equal(rumor.Text, got.Text);
            Assert.Equal(rumor.Time, got.Time);
        }     
        [Fact]
        public async Task HardDeleteRumorAsync_STANDARD()
        {
            await store.HardDeleteRumorAsync(rumor.Id);

            int count = sentry.ExecuteRead(ctx => ctx.Rumors.Count());

            Assert.Equal(0, count);
            _reaper.Deleted(rumor);
        }
        [Fact]
        public async Task HardDeleteRumorAsync_REPORTED()
        {
            RumorReport report = _rumorReportFactory.Create(user, rumor);
            sentry.ExecuteWrite(ctx => ctx.RumorReports.Add(report));

            await store.HardDeleteRumorAsync(rumor.Id);

            int rumorCount = sentry.ExecuteRead(ctx => ctx.Rumors.Count());
            int reportCount = sentry.ExecuteRead(ctx => ctx.RumorReports.Count());

            Assert.Equal(0, rumorCount);
            _reaper.Deleted(rumor);
            Assert.Equal(0, reportCount);
            _reaper.Deleted(report);
        }
        [Fact]
        public async Task HardDeleteRumoredGatheringAsync_STANDARD()
        {
            await store.HardDeleteRumoredGatheringAsync(rumoredGathering.Id);

            int rumorCount = sentry.ExecuteRead(ctx => ctx.Rumors.Count());
            int rumoredGatheringCount = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.Count());

            Assert.Equal(0, rumorCount);
            _reaper.Deleted(rumor);
            Assert.Equal(0, rumoredGatheringCount);
            _reaper.Deleted(rumoredGathering);
        }
        [Fact]
        public async Task HardDeleteRumoredGatheringAsync_INVESTIGATED()
        {
            Investigation investigation = _investigationFactory.Create(user, rumoredGathering);
            sentry.ExecuteWrite(ctx => ctx.Investigations.Add(investigation));

            await store.HardDeleteRumoredGatheringAsync(rumoredGathering.Id);

            int rumorCount = sentry.ExecuteRead(ctx => ctx.Rumors.Count());
            int rumoredGatheringCount = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.Count());
            int investigationCount = sentry.ExecuteRead(ctx => ctx.Investigations.Count());

            Assert.Equal(0, rumorCount);
            _reaper.Deleted(rumor);
            Assert.Equal(0, rumoredGatheringCount);
            _reaper.Deleted(rumoredGathering);
            Assert.Equal(0, investigationCount);
            _reaper.Deleted(investigation);
        }

        [Fact]
        public async Task SoftDeleteRumorAsync_STANDARD()
        {
            await store.SoftDeleteRumorAsync(rumor.Id);

            Rumor softDeletedRumor = sentry.ExecuteRead(ctx => ctx.Rumors.IgnoreQueryFilters().Single());

            Assert.NotNull(softDeletedRumor);
            Assert.Equal(rumor.AuthorId, softDeletedRumor.AuthorId);
            Assert.Equal(rumor.RumoredGatheringId, softDeletedRumor.RumoredGatheringId);
            Assert.Equal(rumor.Text, softDeletedRumor.Text);
            Assert.Equal(rumor.Time, softDeletedRumor.Time);
            Assert.True(softDeletedRumor.SoftDeleted);
        }
        [Fact]
        public async Task SoftDeleteRumorAsync_REPORTED()
        {
            RumorReport report = _rumorReportFactory.Create(user, rumor);
            sentry.ExecuteWrite(ctx => ctx.RumorReports.Add(report));

            await store.SoftDeleteRumorAsync(rumor.Id);

            Rumor softDeletedRumor = sentry.ExecuteRead(ctx => ctx.Rumors.IgnoreQueryFilters().Single());
            RumorReport softDeletedRumorReport = sentry.ExecuteRead(ctx => ctx.RumorReports.IgnoreQueryFilters().Single());

            Assert.NotNull(softDeletedRumor);
            Assert.Equal(rumor.AuthorId, softDeletedRumor.AuthorId);
            Assert.Equal(rumor.RumoredGatheringId, softDeletedRumor.RumoredGatheringId);
            Assert.Equal(rumor.Text, softDeletedRumor.Text);
            Assert.Equal(rumor.Time, softDeletedRumor.Time);
            Assert.True(softDeletedRumor.SoftDeleted);

            Assert.NotNull(softDeletedRumorReport);
            Assert.Equal(report.Id, softDeletedRumorReport.Id);
            Assert.Equal(report.RumorId, softDeletedRumorReport.RumorId);
            Assert.Equal(report.UserId, softDeletedRumorReport.UserId);
            Assert.Equal(report.Notes, softDeletedRumorReport.Notes);
            Assert.Equal(report.Type, softDeletedRumorReport.Type);
            Assert.Equal(report.FilingDate, softDeletedRumorReport.FilingDate);
            Assert.True(softDeletedRumorReport.SoftDeleted);
        }

        [Fact]
        public async Task SoftDeleteRumoredGatheringAsync_STANDARD()
        {
            await store.SoftDeleteRumoredGatheringAsync(rumoredGathering.Id);

            Rumor softDeletedRumor = sentry.ExecuteRead(ctx => ctx.Rumors.IgnoreQueryFilters().Single());
            RumoredGathering softDeletedRumoredGathering = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.IgnoreQueryFilters().Single());

            Assert.NotNull(softDeletedRumor);
            Assert.Equal(rumor.AuthorId, softDeletedRumor.AuthorId);
            Assert.Equal(rumor.RumoredGatheringId, softDeletedRumor.RumoredGatheringId);
            Assert.Equal(rumor.Text, softDeletedRumor.Text);
            Assert.Equal(rumor.Time, softDeletedRumor.Time);
            Assert.True(softDeletedRumor.SoftDeleted);

            Assert.NotNull(softDeletedRumoredGathering);
            Assert.Equal(rumoredGathering.Location.X, softDeletedRumoredGathering.Location.X);
            Assert.Equal(rumoredGathering.Location.Y, softDeletedRumoredGathering.Location.Y);
            Assert.Equal(rumoredGathering.FriendlyLocation, softDeletedRumoredGathering.FriendlyLocation);
            Assert.Equal(rumoredGathering.ConfidenceRating, softDeletedRumoredGathering.ConfidenceRating);
            Assert.True(softDeletedRumoredGathering.SoftDeleted);
        }
        [Fact]
        public async Task SoftDeleteRumoredGatheringAsync_INVESTIGATED()
        {
            Investigation investigation = _investigationFactory.Create(user, rumoredGathering);
            sentry.ExecuteWrite(ctx => ctx.Investigations.Add(investigation));

            await store.SoftDeleteRumoredGatheringAsync(rumoredGathering.Id);

            Rumor softDeletedRumor = sentry.ExecuteRead(ctx => ctx.Rumors.IgnoreQueryFilters().Single());
            Investigation softDeletedInvestigation = sentry.ExecuteRead(ctx => ctx.Investigations.IgnoreQueryFilters().Single());
            RumoredGathering softDeletedRumoredGathering = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.IgnoreQueryFilters().Single());

            Assert.NotNull(softDeletedRumor);
            Assert.Equal(rumor.AuthorId, softDeletedRumor.AuthorId);
            Assert.Equal(rumor.RumoredGatheringId, softDeletedRumor.RumoredGatheringId);
            Assert.Equal(rumor.Text, softDeletedRumor.Text);
            Assert.Equal(rumor.Time, softDeletedRumor.Time);
            Assert.True(softDeletedRumor.SoftDeleted);

            Assert.NotNull(softDeletedInvestigation);
            Assert.Equal(investigation.Id, softDeletedInvestigation.Id);
            Assert.Equal(investigation.Conclusion, softDeletedInvestigation.Conclusion);
            Assert.Equal(investigation.InvestigatorId, softDeletedInvestigation.InvestigatorId);
            Assert.Equal(investigation.RumoredGatheringId, softDeletedInvestigation.RumoredGatheringId);
            Assert.True(softDeletedInvestigation.SoftDeleted);

            Assert.NotNull(softDeletedRumoredGathering);
            Assert.Equal(rumoredGathering.Location.X, softDeletedRumoredGathering.Location.X);
            Assert.Equal(rumoredGathering.Location.Y, softDeletedRumoredGathering.Location.Y);
            Assert.Equal(rumoredGathering.FriendlyLocation, softDeletedRumoredGathering.FriendlyLocation);
            Assert.Equal(rumoredGathering.ConfidenceRating, softDeletedRumoredGathering.ConfidenceRating);
            Assert.True(softDeletedRumoredGathering.SoftDeleted);
        }

        [Fact]
        public async Task UpdateRumorAsync_TEXT()
        {
            string newText = "New Text";
            List<(string, object)> updates = new() { (nameof(rumor.Text), newText) };

            await store.UpdateRumorAsync(rumor.Id, updates);

            Rumor updated = sentry.ExecuteRead(ctx => ctx.Rumors.First());

            Assert.NotNull(updated);
            Assert.Equal(rumor.AuthorId, updated.AuthorId);
            Assert.Equal(rumor.RumoredGatheringId, updated.RumoredGatheringId);
            Assert.NotEqual(rumor.Text, updated.Text);
            Assert.Equal(newText, updated.Text);
            Assert.Equal(rumor.Time, updated.Time);
            Assert.False(updated.SoftDeleted);
        }

        [Fact]
        public async Task UpdateRumorAsync_TIME()
        {
            DateTimeOffset newTime = DateTimeOffset.UtcNow;
            List<(string, object)> updates = new() { (nameof(rumor.Time), newTime) };

            await store.UpdateRumorAsync(rumor.Id, updates);

            Rumor updated = sentry.ExecuteRead(ctx => ctx.Rumors.First());

            Assert.NotNull(updated);
            Assert.Equal(rumor.AuthorId, updated.AuthorId);
            Assert.Equal(rumor.RumoredGatheringId, updated.RumoredGatheringId);
            Assert.Equal(rumor.Text, updated.Text);
            Assert.NotEqual(rumor.Time, updated.Time);
            Assert.Equal(newTime, updated.Time);
            Assert.False(updated.SoftDeleted);
        }

        [Fact]
        public async Task UpdateRumoredGatheringAsync_LOCATION()
        {
            double newLongitude = 90;
            double newLatitude = 90;
            List<(string, object)> updates = new() { (nameof(rumoredGathering.Location), (newLatitude, newLongitude)) };

            await store.UpdateRumoredGatheringAsync(rumoredGathering.Id, updates);

            RumoredGathering updated = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.First());

            Assert.NotNull(updated);
            Assert.NotEqual(rumoredGathering.Location.X, updated.Location.X);
            Assert.NotEqual(rumoredGathering.Location.Y, updated.Location.Y);
            Assert.Equal(newLongitude, updated.Location.X);
            Assert.Equal(newLatitude, updated.Location.Y);
            Assert.Equal(rumoredGathering.FriendlyLocation, updated.FriendlyLocation);
            Assert.Equal(rumoredGathering.ConfidenceRating, updated.ConfidenceRating);
            Assert.False(updated.SoftDeleted);
        }

        [Fact]
        public async Task UpdateRumoredGatheringAsync_CONFIDENCE_RATING()
        {
            int newRating = 30;
            List<(string, object)> updates = new() { (nameof(rumoredGathering.ConfidenceRating), newRating) };

            await store.UpdateRumoredGatheringAsync(rumoredGathering.Id, updates);

            RumoredGathering updated = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.First());

            Assert.NotNull(updated);
            Assert.Equal(rumoredGathering.Location.X, updated.Location.X);
            Assert.Equal(rumoredGathering.Location.Y, updated.Location.Y);
            Assert.Equal(rumoredGathering.FriendlyLocation, updated.FriendlyLocation);
            Assert.NotEqual(rumoredGathering.ConfidenceRating, updated.ConfidenceRating);
            Assert.Equal(newRating, updated.ConfidenceRating);
            Assert.False(updated.SoftDeleted);
        }

        [Fact]
        public async Task UpdateRumoredGatheringAsync_FRIENDLY_LOCATION()
        {
            string newFriendlyLocation = "New Text";
            List<(string, object)> updates = new() { (nameof(rumoredGathering.FriendlyLocation), newFriendlyLocation) };

            await store.UpdateRumoredGatheringAsync(rumoredGathering.Id, updates);

            RumoredGathering updated = sentry.ExecuteRead(ctx => ctx.RumoredGatherings.First());

            Assert.NotNull(updated);
            Assert.Equal(rumoredGathering.Location.X, updated.Location.X);
            Assert.Equal(rumoredGathering.Location.Y, updated.Location.Y);
            Assert.NotEqual(rumoredGathering.FriendlyLocation, updated.FriendlyLocation);
            Assert.Equal(newFriendlyLocation, updated.FriendlyLocation);
            Assert.Equal(rumoredGathering.ConfidenceRating, updated.ConfidenceRating);
            Assert.False(updated.SoftDeleted);
        }

        [Fact]
        public async Task ConfirmRumorAsync_SUCCESS()
        {

        }

        [Fact]
        public async Task DenyRumorAsync_SUCCESS()
        {

        }
        [Fact]
        public async Task GetRumorsByCompanionsOfAsync_SUCCESS()
        {

        }
        [Fact]
        public async Task GetWallRumorsAsync_SUCCESS()
        {

        }
    }
}