using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreDisciplineStore : QueryStore, IDisciplineDatabase
    {
        public EFCoreDisciplineStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.GatheringReport>, List<Core.Boundaries.SnapshotReport>, List<Core.Boundaries.RumorReport>)> GetReportsByUserAsync(long id)
        {
            Task<List<Core.Boundaries.UserReport>> userReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            UserReports.
            Where(r => r.SelfId == id).
            Select(r => new Core.Boundaries.UserReport
            (
                r.Id,
                r.SelfId ?? 0,
                r.OtherId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            Task<List<Core.Boundaries.GatheringReport>> gatheringReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            GatheringReports.
            Where(r => r.UserId == id).
            Select(r => new Core.Boundaries.GatheringReport
            (
                r.Id,
                r.UserId ?? 0,
                r.GatheringId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            Task<List<Core.Boundaries.SnapshotReport>> snapshotReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            SnapshotReports.
            Where(r => r.UserId == id).
            Select(r => new Core.Boundaries.SnapshotReport
            (
                r.Id,
                r.UserId ?? 0,
                r.SnapshotId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            Task<List<Core.Boundaries.RumorReport>> rumorReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            RumorReports.
            Where(r => r.UserId == id).
            Select(r => new Core.Boundaries.RumorReport
            (
                r.Id,
                r.UserId ?? 0,
                r.RumorId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            return (await userReportsToReturn, await gatheringReportsToReturn, await snapshotReportsToReturn, await rumorReportsToReturn);
        }

        public async Task<List<Core.Boundaries.GatheringReport>> GetReportsForGatheringAsync(long id)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.
            GatheringReports.
            Where(r => r.GatheringId == id).
            Select(r => new Core.Boundaries.GatheringReport
            (
                r.Id,
                r.UserId ?? 0,
                r.GatheringId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.GatheringReport>, List<Core.Boundaries.SnapshotReport>, List<Core.Boundaries.RumorReport>)> GetReportsForUserAsync(long id)
        {
            Task<List<Core.Boundaries.UserReport>> userReportsToReturn = storeSentry.ExecuteReadAsync(ctx => 
             ctx.UserReports.
             Where(r => r.OtherId == id).
             Select(r => new Core.Boundaries.UserReport
             (
                 r.Id,
                 r.SelfId ?? 0,
                 r.OtherId,
                 r.FilingDate,
                 r.Type,
                 r.Notes
             )).
            ToListAsync());       

            Task<List<Core.Boundaries.GatheringReport>> gatheringReportsToReturn = storeSentry.ExecuteReadAsync(ctx =>
               ctx.Gatherings.
               Where(g => g.HostId == id).
               Join(
                   ctx.GatheringReports,
                   x => x.HostId,
                   y => y.GatheringId,
                   (x, y) => new Core.Boundaries.GatheringReport
                   (
                       y.Id,
                       y.UserId ?? 0,
                       y.GatheringId,
                       y.FilingDate,
                       y.Type,
                       y.Notes
                   )).
               ToListAsync());

            Task<List<Core.Boundaries.SnapshotReport>> snapshotReportsToReturn = storeSentry.ExecuteReadAsync(ctx =>
               ctx.Snapshots.
               Where(s => s.OwnerId == id).
               Join(
                   ctx.SnapshotReports,
                   x => x.Id,
                   y => y.SnapshotId,
                   (x, y) => new Core.Boundaries.SnapshotReport
                   (
                       y.Id,
                       y.UserId ?? 0,
                       y.SnapshotId,
                       y.FilingDate,
                       y.Type,
                       y.Notes
                   )).
               ToListAsync());

            Task<List<Core.Boundaries.RumorReport>> rumorReportsToReturn = storeSentry.ExecuteReadAsync(ctx =>
               ctx.Rumors.
               Where(s => s.AuthorId == id).
               Join(
                   ctx.RumorReports,
                   x => x.Id,
                   y => y.RumorId,
                   (x, y) => new Core.Boundaries.RumorReport
                   (
                       y.Id, 
                       y.UserId ?? 0, 
                       y.RumorId, 
                       y.FilingDate, 
                       y.Type, 
                       y.Notes
                   )).
               ToListAsync());

            return (await userReportsToReturn, await gatheringReportsToReturn, await snapshotReportsToReturn, await rumorReportsToReturn);
        }

        public async Task ReportGatheringAsync(long userId, long gatheringId, DateTimeOffset timeOfReport, GatheringReportType reportType, string reportDetails)
        {
            GatheringReport toCreate = new()
            {
                UserId = userId,
                GatheringId = gatheringId,
                Type = reportType,
                FilingDate = timeOfReport,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.GatheringReports.Add(toCreate));
        }

        public async Task ReportUserAsync(long userId, long targetUserId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            UserReport toCreate = new()
            {
                SelfId = userId,
                OtherId = targetUserId,
                Type = reportType,
                FilingDate = timeOfReport,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.Add(toCreate));
        }

        public async Task ReportUserAsync(long selfId, long targetId, long gatheringId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            UserReport toCreate = new()
            {
                SelfId = selfId,
                OtherId = targetId,
                GatheringId = gatheringId,
                Type = reportType,
                FilingDate = timeOfReport,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.Add(toCreate));
        }

        public async Task PenaliseUserAsync(long userId, PenaltyType offense, DateTimeOffset timeOfPenalty)
        {
            Penalty toAdd = new() 
            {
                PenalizedId = userId,
                Type = offense, 
                Time = timeOfPenalty 
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Penalties.Add(toAdd));
        }

        public async Task<List<PenaltyShard>> GetPenaltiesForUserAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Penalties.
            Where(p => p.PenalizedId == userId).
            Select(p => new PenaltyShard(p.Type, p.Time)).
            ToListAsync());
        }

        public async Task<List<Core.Boundaries.SnapshotReport>> GetReportsForSnapshotAsync(long snapshotId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.
            SnapshotReports.
            Where(r => r.SnapshotId == snapshotId).
            Select(r => new Core.Boundaries.SnapshotReport
            (
                r.Id,
                r.UserId ?? 0,
                r.SnapshotId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());
        }

        public async Task ReportSnapshotAsync(long userId, long snapshotId, DateTimeOffset timeOfReport, SnapshotReportType reportType, string reportDetails)
        {
            SnapshotReport toCreate = new()
            {
                UserId = userId,
                SnapshotId = snapshotId,
                Type = reportType,
                FilingDate = timeOfReport,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.SnapshotReports.Add(toCreate));
        }

        public Task<List<Core.Boundaries.RumorReport>> GetReportsForRumorAsync(long rumorId)
        {
            return storeSentry.ExecuteReadAsync(ctx => ctx.
            RumorReports.
            Where(r => r.RumorId == rumorId).
            Select(r => new Core.Boundaries.RumorReport
            (
                r.Id,
                r.UserId ?? 0,
                r.RumorId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());
        }

        public async Task ReportRumorAsync(long userId, long rumorId, DateTimeOffset timeOfReport, RumorReportType reportType, string reportDetails)
        {
            RumorReport toCreate = new()
            {
                UserId = userId,
                RumorId = rumorId,
                FilingDate = timeOfReport,
                Notes = reportDetails,
                Type = reportType
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.RumorReports.Add(toCreate));
        }
    }
}
