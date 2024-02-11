using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class DisciplineStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static DisciplineStore store = new DisciplineStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;
        private Event testEvent;

        public DisciplineStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            UserFactory userFactory = new UserFactory();
            subject1 = userFactory.Create();
            subject2 = userFactory.Create();

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject1));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject2));

            EventFactory eventFactory = new EventFactory();
            testEvent = eventFactory.Create(subject2);

            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.Penalties.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Reports.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Events.ExecuteDelete());
        }

        [Fact]
        public async Task GetReportsByUserAsync_SUCCESS()
        {
            UserReport userReport = new UserReportFactory().Create(subject1, subject2, testEvent);
            EventReport eventReport = new EventReportFactory().Create(subject1, testEvent);

            await sentry.ExecuteWriteAsync(ctx => ctx.UserReports.Add(userReport));
            await sentry.ExecuteWriteAsync(ctx => ctx.EventReports.Add(eventReport));

            (List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>) reports = await store.GetReportsByUserAsync(subject1.Id);

            Assert.NotNull(reports.Item1);
            Assert.NotNull(reports.Item2);

            Assert.Equal(userReport.SelfId, reports.Item1.First().ReportingUserId);
            Assert.Equal(userReport.OtherId, reports.Item1.First().ReportedUserId);
            Assert.Equal(userReport.FilingDate, reports.Item1.First().ReportTime);
            Assert.Equal(userReport.Type, reports.Item1.First().ReportType);
            Assert.Equal(userReport.Notes, reports.Item1.First().ReportDetails);

            Assert.Equal(eventReport.SelfId, reports.Item2.First().ReportingUserId);
            Assert.Equal(eventReport.OtherId, reports.Item2.First().ReportedEventHostId);
            Assert.Equal(eventReport.EventId, reports.Item2.First().ReportedEventId);
            Assert.Equal(eventReport.FilingDate, reports.Item2.First().ReportTime);
            Assert.Equal(eventReport.Type, reports.Item2.First().ReportType);
            Assert.Equal(eventReport.Notes, reports.Item2.First().ReportDetails);
        }
        [Fact]
        public async Task ReportUserAsync_SUCCESS()
        {
            string notes = "Test";
            Shared.UserReportType type = Shared.UserReportType.Rude;
            DateTimeOffset time = DateTimeOffset.UtcNow;

            await store.ReportUserAsync(subject1.Id, testEvent.Id, subject2.Id, time, type, notes);

            UserReport created = await sentry.ExecuteReadAsync(ctx => ctx.UserReports.FirstAsync());

            Assert.NotNull(created);
            Assert.Equal(subject1.Id, created.SelfId);
            Assert.Equal(subject2.Id, created.OtherId);
            Assert.Equal(testEvent.Id, created.EventId);
            Assert.Equal(time, created.FilingDate);
            Assert.Equal(type, created.Type);
            Assert.Equal(notes, created.Notes);
        }
        [Fact]
        public async Task ReportEventAsync_SUCCESS()
        {
            string notes = "Test";
            Shared.EventReportType type = Shared.EventReportType.Inappropriate;
            DateTimeOffset time = DateTimeOffset.UtcNow;

            await store.ReportEventAsync(subject1.Id, testEvent.Id, subject2.Id, time, type, notes);

            EventReport created = await sentry.ExecuteReadAsync(ctx => ctx.EventReports.FirstAsync());

            Assert.NotNull(created);
            Assert.Equal(subject1.Id, created.SelfId);
            Assert.Equal(subject2.Id, created.OtherId);
            Assert.Equal(testEvent.Id, created.EventId);
            Assert.Equal(time, created.FilingDate);
            Assert.Equal(type, created.Type);
            Assert.Equal(notes, created.Notes);
        }
        [Fact]
        public async Task GetReportsForEventAsync_SUCCESS()
        {
            EventReport eventReport = new EventReportFactory().Create(subject1, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventReports.Add(eventReport));

            List<Core.Boundaries.EventReport> reports = await store.GetReportsForEventAsync(testEvent.Id);

            Assert.NotNull(reports);
            Assert.Equal(eventReport.SelfId, reports.First().ReportingUserId);
            Assert.Equal(eventReport.OtherId, reports.First().ReportedEventHostId);
            Assert.Equal(eventReport.EventId, reports.First().ReportedEventId);
            Assert.Equal(eventReport.FilingDate, reports.First().ReportTime);
            Assert.Equal(eventReport.Type, reports.First().ReportType);
            Assert.Equal(eventReport.Notes, reports.First().ReportDetails);
        }
        [Fact]
        public async Task GetReportsForUserAsync_SUCCESS()
        {
            UserReport userReport = new UserReportFactory().Create(subject1, subject2, testEvent);
            EventReport eventReport = new EventReportFactory().Create(subject1, testEvent);

            await sentry.ExecuteWriteAsync(ctx => ctx.UserReports.Add(userReport));
            await sentry.ExecuteWriteAsync(ctx => ctx.EventReports.Add(eventReport));

            (List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>) reports = await store.GetReportsForUserAsync(subject2.Id);

            Assert.NotNull(reports.Item1);
            Assert.NotNull(reports.Item2);

            Assert.Equal(userReport.SelfId, reports.Item1.First().ReportingUserId);
            Assert.Equal(userReport.OtherId, reports.Item1.First().ReportedUserId);
            Assert.Equal(userReport.FilingDate, reports.Item1.First().ReportTime);
            Assert.Equal(userReport.Type, reports.Item1.First().ReportType);
            Assert.Equal(userReport.Notes, reports.Item1.First().ReportDetails);

            Assert.Equal(eventReport.SelfId, reports.Item2.First().ReportingUserId);
            Assert.Equal(eventReport.OtherId, reports.Item2.First().ReportedEventHostId);
            Assert.Equal(eventReport.EventId, reports.Item2.First().ReportedEventId);
            Assert.Equal(eventReport.FilingDate, reports.Item2.First().ReportTime);
            Assert.Equal(eventReport.Type, reports.Item2.First().ReportType);
            Assert.Equal(eventReport.Notes, reports.Item2.First().ReportDetails);
        }
        [Fact]
        public async Task PenaliseUserAsync_SUCCESS()
        {
            await store.PenaliseUserAsync(subject1.Id, PenaltyType.Unreliable, DateTimeOffset.MinValue);

            Entities.Penalty penalty = sentry.ExecuteRead(ctx => ctx.Penalties.Single());

            Assert.NotNull(penalty);
            Assert.Equal(subject1.Id, penalty.PenalizedId);
            Assert.Equal(DateTimeOffset.MinValue, penalty.Time);
            Assert.Equal(PenaltyType.Unreliable, penalty.Type);
        }
        [Fact]
        public async Task GetPenaltiesForUserAsync_SUCCESS()
        {
            Entities.Penalty penalty = new PenaltyFactory().Create(subject1);
            sentry.ExecuteWrite(ctx => ctx.Penalties.Add(penalty));

            Penalty found = (await store.GetPenaltiesForUserAsync(subject1.Id)).Single();

            Assert.NotNull(found);
            Assert.Equal(DateTimeOffset.MinValue, found.TimeOfPenalty);
            Assert.Equal(PenaltyType.Unreliable, found.Offense);
        }
    }
}
