using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests
{
    public class ReportStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static ReportStore reportStore = new ReportStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;
        private Event testEvent;

        public ReportStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            UserFactory userFactory = new UserFactory();
            subject1 = userFactory.Create();
            subject2 = userFactory.Create();

            sentry.ExecuteWriteAsync(ctx => ctx.Users.AddAsync(subject1));
            sentry.ExecuteWriteAsync(ctx => ctx.Users.AddAsync(subject2));

            EventFactory eventFactory = new EventFactory();
            testEvent = eventFactory.Create(subject2);

            sentry.ExecuteWriteAsync(ctx => ctx.Events.AddAsync(testEvent));
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetReportsByUserAsync_SUCCESS()
        {
            UserReport userReport = new UserReportFactory().Create(subject1, subject2, testEvent);
            EventReport eventReport = new EventReportFactory().Create(subject1, testEvent);

            await sentry.ExecuteWriteAsync(ctx => ctx.UserReports.AddAsync(userReport));
            await sentry.ExecuteWriteAsync(ctx => ctx.EventReports.AddAsync(eventReport));

            (List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>) reports = await reportStore.GetReportsByUserAsync(subject1.Id);

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
            Shared.UserReportType type = Shared.UserReportType.rude;

            await reportStore.ReportUserAsync(subject1.Id, testEvent.Id, subject2.Id, type, notes);

            UserReport created = await sentry.ExecuteReadAsync(ctx => ctx.UserReports.FirstAsync());
      
            Assert.NotNull(created);
            Assert.Equal(subject1.Id, created.SelfId);
            Assert.Equal(subject2.Id, created.OtherId);
            Assert.Equal(testEvent.Id, created.EventId);
            Assert.Equal(type, created.Type);
            Assert.Equal(notes, created.Notes);       
        }
        [Fact]
        public async Task ReportEventAsync_SUCCESS()
        {
            string notes = "Test";
            Shared.EventReportType type = Shared.EventReportType.inappropriate;

            await reportStore.ReportEventAsync(subject1.Id, testEvent.Id, subject2.Id, type, notes);

            EventReport created = await sentry.ExecuteReadAsync(ctx => ctx.EventReports.FirstAsync());

            Assert.NotNull(created);
            Assert.Equal(subject1.Id, created.SelfId);
            Assert.Equal(subject2.Id, created.OtherId);
            Assert.Equal(testEvent.Id, created.EventId);
            Assert.Equal(type, created.Type);
            Assert.Equal(notes, created.Notes);
        }
        [Fact]
        public async Task GetReportsForEventAsync_SUCCESS()
        {
            EventReport eventReport = new EventReportFactory().Create(subject1, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.EventReports.AddAsync(eventReport));

            List<Core.Boundaries.EventReport> reports = await reportStore.GetReportsForEventAsync(testEvent.Id);

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

            await sentry.ExecuteWriteAsync(ctx => ctx.UserReports.AddAsync(userReport));
            await sentry.ExecuteWriteAsync(ctx => ctx.EventReports.AddAsync(eventReport));

            (List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>) reports = await reportStore.GetReportsForUserAsync(subject2.Id);

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
    }
}
