

using Core.Boundaries;
using Repository.Tests.Factories;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Repository.Tests
{
    public class ReportStoreTests
    {
        private static TestSentry sentry = new TestSentry();
        private static ReportStore reportStore = new ReportStore(sentry);
        private static EventStore store = new EventStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject1;
        private User subject2;

        private User testUser;
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
            testEvent = eventFactory.Create();
            testEvent.HostId = subject2.Id;

            sentry.ExecuteWriteAsync(ctx => ctx.Events.AddAsync(testEvent));
        }

        [Fact]
        public async Task GetReportsByUserAsync_SUCCESS()
        {
            ReportFactory reportFactory = new ReportFactory();
            Report first = reportFactory.Create(subject1, subject2, testEvent);
            Report second = reportFactory.Create(subject1, subject2, testEvent);

            await sentry.ExecuteWriteAsync(ctx => ctx.Reports.AddAsync(first));
            await sentry.ExecuteWriteAsync(ctx => ctx.Reports.AddAsync(second));

            (List<UserReport>, List<EventReport>) reports = await reportStore.GetReportsByUserAsync(new Guid());


        }
        [Fact]
        public async Task ReportUserAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task ReportEventAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetReportsForEventAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetReportsForUserAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}
