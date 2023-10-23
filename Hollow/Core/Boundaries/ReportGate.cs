using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    public record UserReport(Guid Id, Guid ReportingUserId, Guid ReportedUserId, DateTimeOffset ReportTime,
        UserReportType ReportType, string ReportDetails);

    public record EventReport(Guid Id, Guid ReportingUserId, Guid ReportedEventId,
        Guid ReportedEventHostId, DateTimeOffset ReportTime,
        EventReportType ReportType, string ReportDetails);

    public interface IReportDatabase
    {
        (List<UserReport>, List<EventReport>) GetReportsForUser(Guid id);
        (List<UserReport>, List<EventReport>) GetReportsByUser(Guid id);
        bool ReportUser(Guid selfId, Guid eventId, Guid targetId,
            UserReportType reportType, string reportDetails);

        List<EventReport> GetReportsForEvent(Guid id);
        bool ReportEvent(Guid userId, Guid eventId, Guid HostId,
            EventReportType reportType, string reportDetails);
    }

    public interface IReportOperations
    {
        Task ReportUserAsync(Guid userID, Guid targetID,
            UserReportType reportType, string reportDetails);

        Task ReportEventAsync(Guid userID, Guid eventID, Guid hostId,
            EventReportType reportType, string reportDetails);
    }
}

