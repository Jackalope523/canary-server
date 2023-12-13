

using static Repository.Report;

namespace Repository.Tests.Factories
{
    internal class ReportFactory
    {
        private int created = 0;

        public Report Create(User reporter, User reportee, Event location)
        {
            created++;
            return new Report
            {
                SelfId = reporter.Id,
                OtherId = reportee.Id,
                EventId = location.Id,
                Type = Report.ReportType.Rude,
                FilingDate = DateTimeOffset.Now,
                Notes = "Test Report " + created
            };          
        }
    }
}
