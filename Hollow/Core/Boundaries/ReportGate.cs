using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

	public record UserReport(ulong Id, ulong ReportingUserId, ulong ReportedUserId, DateTimeOffset ReportTime,
        UserReportType ReportType, string ReportDetails);

    public record EventReport(ulong Id, ulong ReportingUserId, ulong ReportedEventId,
        ulong ReportedEventHostId, DateTimeOffset ReportTime,
        EventReportType ReportType, string ReportDetails);

	#endregion

	#region Gates

	public interface IReportDatabase
    {
        (List<UserReport>, List<EventReport>) GetReportsForUser(ulong userId);
        (List<UserReport>, List<EventReport>) GetReportsByUser(ulong userId);
        bool ReportUser(ulong userId, ulong eventId, ulong targetUserId,
            UserReportType reportType, string reportDetails);

        List<EventReport> GetReportsForEvent(ulong eventId);
        bool ReportEvent(ulong userId, ulong eventId, ulong hostId,
            EventReportType reportType, string reportDetails);
    }

    public interface IReportOperations
    {
        Task ReportUserAsync(ulong userId, ulong targetId,
            UserReportType reportType, string reportDetails);

        Task ReportEventAsync(ulong userId, ulong eventId, ulong hostId,
            EventReportType reportType, string reportDetails);
    }

	#endregion
}

