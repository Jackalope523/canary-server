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

        public (List<UserReport>, List<EventReport>) GetReportsByUser(Guid id)
        {
            List<Report> reports = getReports(r => r.SelfId == id);
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

        public List<EventReport> GetReportsForEvent(Guid id)
        {
            List<Report> reports = getReports(r => r.EventId == id);
            List<EventReport> toReturn = new List<EventReport>();

            foreach (Report report in reports)
            {
                toReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));
            }

            return toReturn;
        }

        public (List<UserReport>, List<EventReport>) GetReportsForUser(Guid id)
        {
            List<Report> userReports = getReports(r => r.OtherId == id);
            List<Report> eventReports = getReports(r => r.OtherId == id && r.OtherId == r.Event.HostId);
            List<UserReport> userReportsToReturn = new List<UserReport>();
            List<EventReport> eventReportsToReturn = new List<EventReport>();

            foreach (Report report in userReports)
            {
                userReportsToReturn.Add(new UserReport(report.Id, report.SelfId, report.OtherId, report.FilingDate, Report.ToUserReportType(report.Type), report.Notes));
            }
            foreach (Report report in eventReports)
            {
                eventReportsToReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));

            }

            return (userReportsToReturn, eventReportsToReturn);
        }

        public bool ReportEvent(Guid userId, Guid eventId, Guid HostId, EventReportType reportType, string reportDetails)
        {

            return CreateReport(userId, eventId, HostId, Report.ToReportType(reportType), DateTimeOffset.Now, reportDetails);
        }

        public bool ReportUser(Guid selfId, Guid eventId, Guid targetId, UserReportType reportType, string reportDetails)
        {
            return CreateReport(selfId, eventId, targetId, Report.ToReportType(reportType), DateTimeOffset.Now, reportDetails);
        }
    }
}
