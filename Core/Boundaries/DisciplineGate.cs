using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

    public enum PenaltyType
    { Unreliable }

    public enum EventReportType
    { Inappropriate, Spam, Misleading, Promotion }

    public enum UserReportType
    {
        Rude, HateSpeech, Harassment,
        Violent, Assault
    }

    public record PenaltyShard(PenaltyType Offense, DateTimeOffset TimeOfPenalty);

	public record UserReport(ulong Id, ulong ReportingUserId, ulong ReportedUserId, DateTimeOffset ReportTime,
        UserReportType ReportType, string ReportDetails);

    public record EventReport(ulong Id, ulong ReportingUserId, ulong ReportedEventId, DateTimeOffset ReportTime,
        EventReportType ReportType, string ReportDetails);

	#endregion

	#region Gates

	public interface IDisciplineDatabase
    {
        Task<List<PenaltyShard>> GetPenaltiesForUserAsync(ulong userId);
        Task PenaliseUserAsync(ulong userId, PenaltyType offense, DateTimeOffset timeOfPenalty);

        Task<(List<UserReport>, List<EventReport>)> GetReportsForUserAsync(ulong userId);
        Task<(List<UserReport>, List<EventReport>)> GetReportsByUserAsync(ulong userId);
        Task ReportUserAsync(ulong userId, ulong targetUserId, ulong eventId, DateTimeOffset timeOfReport,
            UserReportType reportType, string reportDetails);
        Task ReportUserAsync(ulong userId, ulong targetUserId, DateTimeOffset timeOfReport,
            UserReportType reportType, string reportDetails);

        Task<List<EventReport>> GetReportsForEventAsync(ulong eventId);
        Task ReportEventAsync(ulong userId, ulong eventId, DateTimeOffset timeOfReport,
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

