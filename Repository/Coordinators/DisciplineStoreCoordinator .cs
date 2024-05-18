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

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsByUserAsync(ulong id)
        {
            return await store.GetReportsByUserAsync(id);
        }

        public async Task<List<Core.Boundaries.EventReport>> GetReportsForEventAsync(ulong id)
        {
            return await store.GetReportsForEventAsync(id);
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsForUserAsync(ulong id)
        {
            return await store.GetReportsForUserAsync(id);
        }

        public async Task ReportEventAsync(ulong userId, ulong eventId, DateTimeOffset timeOfReport, EventReportType reportType, string reportDetails)
        {
            await store.ReportEventAsync(userId, eventId, timeOfReport, reportType, reportDetails);
        }

        public async Task ReportUserAsync(ulong userId, ulong targetUserId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            await store.ReportUserAsync(userId, targetUserId, timeOfReport, reportType, reportDetails);
        }

        public async Task ReportUserAsync(ulong selfId, ulong targetId, ulong eventId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            await store.ReportUserAsync(selfId, targetId, timeOfReport, reportType, reportDetails);
        }

        public async Task PenaliseUserAsync(ulong userId, PenaltyType offense, DateTimeOffset timeOfPenalty)
        {
            await store.PenaliseUserAsync(userId, offense, timeOfPenalty);
        }

        public async Task<List<PenaltyShard>> GetPenaltiesForUserAsync(ulong userId)
        {
            return await store.GetPenaltiesForUserAsync(userId);
        }     
    }
}
