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
        Task<(List<UserReport>, List<EventReport>)> GetReportsForUserAsync(Guid id);
        Task<(List<UserReport>, List<EventReport>)> GetReportsByUserAsync(Guid id);
        Task<bool> ReportUserAsync(Guid selfId, Guid eventId, Guid targetId,
            UserReportType reportType, string reportDetails);

        Task<List<EventReport>> GetReportsForEventAsync(Guid id);
        Task<bool> ReportEventAsync(Guid userId, Guid eventId, Guid HostId,
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

