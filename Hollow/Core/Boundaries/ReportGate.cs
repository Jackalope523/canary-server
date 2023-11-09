using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    public record UserReport(ulong Id, ulong ReportingUserId, ulong ReportedUserId, DateTimeOffset ReportTime,
        UserReportType ReportType, string ReportDetails);

    public record EventReport(ulong Id, ulong ReportingUserId, ulong ReportedEventId,
        ulong ReportedEventHostId, DateTimeOffset ReportTime,
        EventReportType ReportType, string ReportDetails);

    public interface IReportDatabase
    {
        (List<UserReport>, List<EventReport>) GetReportsForUser(ulong id);
        (List<UserReport>, List<EventReport>) GetReportsByUser(ulong id);
        bool ReportUser(ulong selfId, ulong eventId, ulong targetId,
            UserReportType reportType, string reportDetails);

        List<EventReport> GetReportsForEvent(ulong id);
        bool ReportEvent(ulong userId, ulong eventId, ulong HostId,
            EventReportType reportType, string reportDetails);
    }

    public interface IReportOperations
    {
        Task ReportUserAsync(ulong userID, ulong targetID,
            UserReportType reportType, string reportDetails);

        Task ReportEventAsync(ulong userID, ulong eventID, ulong hostId,
            EventReportType reportType, string reportDetails);
    }
}

