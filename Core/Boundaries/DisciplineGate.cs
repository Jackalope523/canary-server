using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

    public enum PenaltyType
    { Unreliable }

    public enum UserReportType
    {
        Rude, HateSpeech, Harassment,
        ViolenceOrAssault, Other
    }

    public enum GatheringReportType
    {
        Inappropriate, Misleading,
        Promotion, Spam, Other
    }

    public enum SnapshotReportType
    {
        Inappropriate, GraphicContent, ManipulatedMedia,
        Promotion, Spam, Other
    }

    public record PenaltyShard(PenaltyType Offense, DateTimeOffset TimeOfPenalty);

	public record UserReport(ulong Id, ulong ReportingUserId, ulong ReportedUserId, DateTimeOffset ReportTime,
        UserReportType ReportType, string ReportDetails);

    public record GatheringReport(ulong Id, ulong ReportingUserId, ulong ReportedGatheringId, DateTimeOffset ReportTime,
        GatheringReportType ReportType, string ReportDetails);

    public record SnapshotReport(ulong Id, ulong ReportingUserId, ulong ReportedSnapshotId, DateTimeOffset ReportTime,
        SnapshotReportType ReportType, string ReportDetails);

	#endregion

	#region Gates

	public interface IDisciplineDatabase
    {
        Task<List<PenaltyShard>> GetPenaltiesForUserAsync(ulong userId);
        Task PenaliseUserAsync(ulong userId, PenaltyType offense, DateTimeOffset timeOfPenalty);

        Task<(List<UserReport>, List<GatheringReport>, List<SnapshotReport>)> GetReportsForUserAsync(ulong userId);
        Task<(List<UserReport>, List<GatheringReport>, List<SnapshotReport>)> GetReportsByUserAsync(ulong userId);
        Task ReportUserAsync(ulong userId, ulong targetUserId, ulong gatheringId, DateTimeOffset timeOfReport,
            UserReportType reportType, string reportDetails);
        Task ReportUserAsync(ulong userId, ulong targetUserId, DateTimeOffset timeOfReport,
            UserReportType reportType, string reportDetails);

        Task<List<GatheringReport>> GetReportsForGatheringAsync(ulong gatheringId);
        Task ReportGatheringAsync(ulong userId, ulong gatheringId, DateTimeOffset timeOfReport,
            GatheringReportType reportType, string reportDetails);

        Task<List<SnapshotReport>> GetReportsForSnapshotAsync(ulong snapshotId);
        Task ReportSnapshotAsync(ulong userId, ulong snapshotId, DateTimeOffset timeOfReport,
            SnapshotReportType reportType, string reportDetails);
    }

    public interface IDisciplineOperations
    {
        Task ReportUserAsync(ulong userId, ulong targetId,
            UserReportType reportType, string reportDetails);

        Task ReportGatheringAsync(ulong userId, ulong gatheringId,
            GatheringReportType reportType, string reportDetails);

        Task ReportSnapshotAsync(ulong userId, ulong snapshotId,
            SnapshotReportType reportType, string reportDetails);
    }

	#endregion
}

