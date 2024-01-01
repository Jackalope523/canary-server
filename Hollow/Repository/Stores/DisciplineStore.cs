using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository
{
    public class DisciplineStore : QueryStore, IDisciplineDatabase
    {
        public static IDisciplineDatabase ReportDatabaseAccess => new DisciplineStore(new TestSentry());

        public DisciplineStore(Sentry sentry) : base(sentry)
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
            Where(r => r.SelfId == id).
            Select(r => new Core.Boundaries.EventReport
            (
                r.Id,
                r.SelfId,
                r.EventId,
                r.OtherId,
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
                r.SelfId,
                r.EventId,
                r.OtherId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsForUserAsync(ulong id)
        {
            Task<List<Core.Boundaries.UserReport>> userReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
             UserReports.
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
            Task<List<Core.Boundaries.EventReport>> eventReportsToReturn = storeSentry.ExecuteReadAsync(ctx => ctx.
            EventReports.
            Where(r => r.OtherId == id).
            Select(r => new Core.Boundaries.EventReport
            (
                r.Id,
                r.SelfId,
                r.EventId,
                r.OtherId,
                r.FilingDate,
                r.Type,
                r.Notes
            )).
            ToListAsync());

            return (await userReportsToReturn, await eventReportsToReturn);
        }

        public async Task<bool> ReportEventAsync(ulong userId, ulong eventId, ulong HostId, EventReportType reportType, string reportDetails)
        {
            EventReport toCreate = new()
            {
                SelfId = userId,
                OtherId = HostId,
                EventId = eventId,
                Type = reportType,
                FilingDate = DateTime.UtcNow,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventReports.Add(toCreate));
            return true;
        }

        public async Task<bool> ReportUserAsync(ulong selfId, ulong eventId, ulong targetId, UserReportType reportType, string reportDetails)
        {
            UserReport toCreate = new()
            {
                SelfId = selfId,
                OtherId = targetId,
                EventId = eventId,
                Type = reportType,
                FilingDate = DateTime.UtcNow,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.Add(toCreate));
            return true;
        }

        public async Task<bool> PenaliseUserAsync(ulong userId, PenaltyType offense, DateTimeOffset timeOfPenalty)
        {
            Entities.Penalty toAdd = new() 
            {
                PenalizedId = userId,
                Type = offense, 
                Time = timeOfPenalty 
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Penalties.Add(toAdd));
            return true;
        }

        public async Task<List<Penalty>> GetPenaltiesForUserAsync(ulong userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Penalties.
            Where(p => p.PenalizedId == userId).
            Select(p => new Penalty(p.Type, p.Time)).
            ToListAsync());
        }      
    }
}
