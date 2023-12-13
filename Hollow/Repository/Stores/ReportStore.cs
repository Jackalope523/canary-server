using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository
{
    public class ReportStore : QueryStore, IReportDatabase
    {
        public static IReportDatabase ReportDatabaseAccess => new ReportStore(new TestSentry());

        public ReportStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsByUserAsync(Guid id)
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

        public async Task<List<Core.Boundaries.EventReport>> GetReportsForEventAsync(Guid id)
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

        public async Task<(List<Core.Boundaries.UserReport>, List<Core.Boundaries.EventReport>)> GetReportsForUserAsync(Guid id)
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

        public async Task<bool> ReportEventAsync(Guid userId, Guid eventId, Guid HostId, EventReportType reportType, string reportDetails)
        {
            EventReport toCreate = new EventReport
            {
                SelfId = userId,
                OtherId = HostId,
                EventId = eventId,
                Type = reportType,
                FilingDate = DateTime.UtcNow,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventReports.AddAsync(toCreate));
            return true;
        }

        public async Task<bool> ReportUserAsync(Guid selfId, Guid eventId, Guid targetId, UserReportType reportType, string reportDetails)
        {
            UserReport toCreate = new UserReport
            {
                SelfId = selfId,
                OtherId = targetId,
                EventId = eventId,
                Type = reportType,
                FilingDate = DateTime.UtcNow,
                Notes = reportDetails
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserReports.AddAsync(toCreate));
            return true;
        }      
    }
}
