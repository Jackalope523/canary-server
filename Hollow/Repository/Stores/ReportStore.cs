using Core.Boundaries;
using Shared;

namespace Repository
{
    public class ReportStore : QueryStore, IReportDatabase
    {
        public static IReportDatabase ReportDatabaseAccess => new ReportStore(new TestSentry());

        public ReportStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<(List<UserReport>, List<EventReport>)> GetReportsByUserAsync(Guid id)
        {
            List<Report> reports = await GetReportsAsync(r => r.SelfId == id);
            List<UserReport> userReportsToReturn = new List<UserReport>();
            List<EventReport> eventReportsToReturn = new List<EventReport>();

            foreach (Report report in reports)
            {
                if (report.OtherId != report.Event.HostId)
                {
                    userReportsToReturn.Add(new UserReport(report.Id, report.SelfId, report.OtherId, report.FilingDate, Report.ToUserReportType(report.Type), report.Notes));
                }
                else
                {
                    eventReportsToReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));
                }

            }
            return (userReportsToReturn, eventReportsToReturn);
        }

        public async Task<List<EventReport>> GetReportsForEventAsync(Guid id)
        {
            List<Report> reports = await GetReportsAsync(r => r.EventId == id);
            List<EventReport> toReturn = new List<EventReport>();

            foreach (Report report in reports)
            {
                toReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));
            }

            return toReturn;
        }

        public async Task<(List<UserReport>, List<EventReport>)> GetReportsForUserAsync(Guid id)
        {
            Task<List<Report>> userReports = GetReportsAsync(r => r.OtherId == id);
            Task<List<Report>> eventReports = GetReportsAsync(r => r.OtherId == id && r.OtherId == r.Event.HostId);
            List<UserReport> userReportsToReturn = new List<UserReport>();
            List<EventReport> eventReportsToReturn = new List<EventReport>();

            List<Report> a = await userReports;
            List<Report> b = await eventReports;
            foreach (Report report in a)
            {
                userReportsToReturn.Add(new UserReport(report.Id, report.SelfId, report.OtherId, report.FilingDate, Report.ToUserReportType(report.Type), report.Notes));
            }
            foreach (Report report in b)
            {
                eventReportsToReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));

            }

            return (userReportsToReturn, eventReportsToReturn);
        }

        public async Task<bool> ReportEventAsync(Guid userId, Guid eventId, Guid HostId, EventReportType reportType, string reportDetails)
        {

            return await CreateReportAsync(userId, eventId, HostId, Report.ToReportType(reportType), DateTimeOffset.Now, reportDetails);
        }

        public async Task<bool> ReportUserAsync(Guid selfId, Guid eventId, Guid targetId, UserReportType reportType, string reportDetails)
        {
            return await CreateReportAsync(selfId, eventId, targetId, Report.ToReportType(reportType), DateTimeOffset.Now, reportDetails);
        }
    }
}
