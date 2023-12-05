using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

    public enum PenaltyType
    { }

    public record Penalty(PenaltyType Offense, DateTimeOffset TimeOfPenalty);

	public record UserReport(ulong Id, ulong ReportingUserId, ulong ReportedUserId, DateTimeOffset ReportTime,
        UserReportType ReportType, string ReportDetails);

    public record EventReport(ulong Id, ulong ReportingUserId, ulong ReportedEventId,
        ulong ReportedEventHostId, DateTimeOffset ReportTime,
        EventReportType ReportType, string ReportDetails);

	#endregion

	#region Gates

	public interface IDisciplineDatabase
    {
        Task<List<Penalty>> GetPenaltiesForUserAsync(ulong userId);
        Task<bool> PenaliseUserAsync(ulong userId, Penalty penalty);

        Task<(List<UserReport>, List<EventReport>)> GetReportsForUserAsync(ulong userId);
        Task<(List<UserReport>, List<EventReport>)> GetReportsByUserAsync(ulong userId);
        Task<bool> ReportUserAsync(ulong userId, ulong eventId, ulong targetUserId,
            UserReportType reportType, string reportDetails);

        Task<List<EventReport>> GetReportsForEventAsync(ulong eventId);
        Task<bool> ReportEventAsync(ulong userId, ulong eventId, ulong hostId,
            EventReportType reportType, string reportDetails);
    }

    public interface IDisciplineOperations
    {
        Task ReportUserAsync(ulong userId, ulong targetId,
            UserReportType reportType, string reportDetails);

        Task ReportEventAsync(ulong userId, ulong eventId,
            EventReportType reportType, string reportDetails);
    }

	#endregion
}

