using Core.Boundaries;

namespace Repository
{
    public class DisciplineStoreCoordinator : IDisciplineDatabase
    {
        private readonly IDisciplineDatabase store;

        public DisciplineStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreDisciplineStore(flag);
        }

        public async Task<List<Core.Boundaries.GatheringReport>> GetReportsForGatheringAsync(long id)
        {
            return await store.GetReportsForGatheringAsync(id);
        }

        public async Task ReportGatheringAsync(long userId, long gatheringId, DateTimeOffset timeOfReport, GatheringReportType reportType, string reportDetails)
        {
            await store.ReportGatheringAsync(userId, gatheringId, timeOfReport, reportType, reportDetails);
        }

        public async Task ReportUserAsync(long userId, long targetUserId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            await store.ReportUserAsync(userId, targetUserId, timeOfReport, reportType, reportDetails);
        }

        public async Task ReportUserAsync(long selfId, long targetId, long gatheringId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            await store.ReportUserAsync(selfId, targetId, timeOfReport, reportType, reportDetails);
        }

        public async Task PenaliseUserAsync(long userId, PenaltyType offense, DateTimeOffset timeOfPenalty)
        {
            await store.PenaliseUserAsync(userId, offense, timeOfPenalty);
        }

        public async Task<List<PenaltyShard>> GetPenaltiesForUserAsync(long userId)
        {
            return await store.GetPenaltiesForUserAsync(userId);
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.GatheringReport>, List<Core.Boundaries.SnapshotReport>, List<Core.Boundaries.RumorReport>)> GetReportsForUserAsync(long userId)
        {
            return await store.GetReportsForUserAsync(userId);
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.GatheringReport>, List<Core.Boundaries.SnapshotReport>, List<Core.Boundaries.RumorReport>)> GetReportsByUserAsync(long userId)
        {
            return await store.GetReportsByUserAsync(userId);
        }

        public async Task<List<Core.Boundaries.SnapshotReport>> GetReportsForSnapshotAsync(long snapshotId)
        {
            return await store.GetReportsForSnapshotAsync(snapshotId);
        }

        public async Task ReportSnapshotAsync(long userId, long snapshotId, DateTimeOffset timeOfReport, SnapshotReportType reportType, string reportDetails)
        {
            await store.ReportSnapshotAsync(userId, snapshotId, timeOfReport, reportType, reportDetails);
        }

        public async Task<List<Core.Boundaries.RumorReport>> GetReportsForRumorAsync(long rumorId)
        {
            return await store.GetReportsForRumorAsync(rumorId);
        }

        public async Task ReportRumorAsync(long userId, long rumorId, DateTimeOffset timeOfReport, RumorReportType reportType, string reportDetails)
        {
            await store.ReportRumorAsync(userId, rumorId, timeOfReport, reportType, reportDetails);
        }
    }
}
