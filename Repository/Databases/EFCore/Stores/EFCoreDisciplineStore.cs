using Core.Boundaries;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class EFCoreDisciplineStore : QueryStore, IDisciplineDatabase
    {
        public EFCoreDisciplineStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsByUserAsync(ulong id)
        {
            Task<List<Core.Boundaries.UserReport>> userReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            UserReports.
            Where(r => r.SelfId == id).
            Select(r => new Core.Boundaries.UserReport
            (
                r.Id,
                r.SelfId,
                r.OtherId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            Task<List<Core.Boundaries.EventReport>> eventReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            EventReports.
            Where(r => r.UserId == id).
            Select(r => new Core.Boundaries.EventReport
            (
                r.Id,
                r.UserId,
                r.EventId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());
         
            return (await userReportsToReturn, await eventReportsToReturn);
        }

        public async Task<List<Core.Boundaries.EventReport>> GetReportsForEventAsync(ulong id)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.
            EventReports.
            Where(r => r.EventId == id).
            Select(r => new Core.Boundaries.EventReport
            (
                r.Id,
                r.UserId,
                r.EventId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsForUserAsync(ulong id)
        {
            Task<List<Core.Boundaries.UserReport>> userReportsToReturn = storeSentry.ExecuteReadAsync(ctx => 
             ctx.UserReports.
             Where(r => r.OtherId == id).
             Select(r => new Core.Boundaries.UserReport
             (
                 r.Id,
                 r.SelfId,
                 r.OtherId,
                 r.FilingDate,
                 r.Type,
                 r.Notes
             )).
            ToListAsync());       

            List<ulong> eventsHosted = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Events.
                Where(e => e.HostId == id).
                Select(e => e.Id).
                ToListAsync());

            Task<List<Core.Boundaries.EventReport>>  eventReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            EventReports.
            Where(r => eventsHosted.Contains(r.EventId)).
            Select(r => new Core.Boundaries.EventReport
            (
                r.Id,
                r.UserId,
                r.EventId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            return (await userReportsToReturn, await eventReportsToReturn);
        }

        public async Task ReportEventAsync(ulong userId, ulong eventId, DateTimeOffset timeOfReport, EventReportType reportType, string reportDetails)
        {
            EventReport toCreate = new()
            {
                UserId = userId,
                EventId = eventId,
                Type = reportType,
                FilingDate = timeOfReport,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventReports.Add(toCreate));
        }

        public async Task ReportUserAsync(ulong userId, ulong targetUserId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
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

        public async Task ReportUserAsync(ulong selfId, ulong targetId, ulong eventId, DateTimeOffset timeOfReport, UserReportType reportType, string reportDetails)
        {
            UserReport toCreate = new()
            {
                SelfId = selfId,
                OtherId = targetId,
                EventId = eventId,
                Type = reportType,
                FilingDate = timeOfReport,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.Add(toCreate));
        }

        public async Task PenaliseUserAsync(ulong userId, PenaltyType offense, DateTimeOffset timeOfPenalty)
        {
            Entities.Penalty toAdd = new() 
            {
                PenalizedId = userId,
                Type = offense, 
                Time = timeOfPenalty 
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Penalties.Add(toAdd));
        }

        public async Task<List<PenaltyShard>> GetPenaltiesForUserAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Penalties.
            Where(p => p.PenalizedId == userId).
            Select(p => new PenaltyShard(p.Type, p.Time)).
            ToListAsync());
        }     
    }
}
