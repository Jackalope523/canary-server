using Microsoft.EntityFrameworkCore;
using Core.Boundaries;

namespace Repository
{
    public abstract class QueryStore
    {
        protected Sentry storeSentry;

        public QueryStore(Sentry sentry)
        {
            storeSentry = sentry;
        }

        // UTIL      
        protected async Task<bool> AddLinkOperationAsync(Link link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Links.Add(link));
            return true;

        }
        protected async Task<bool> RemoveLinkOperationAsync(UserLink link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
            return true;
        }
        protected async Task<bool> RemoveLinkOperationAsync(EventLink link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
            return true;
        }
        protected async Task<bool> RemoveLinkOperationAsync(PostLink link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.PostLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
            return true;
        }
       
        protected async Task<List<UserSilhouette>> GetUsersByAsync(Func<UserLink, bool> predicate)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => predicate(l)).Select(l => new UserSilhouette(l.Other.Id, l.Other.Name)).ToListAsync());
        }

        protected async Task<List<Report>> GetReportsAsync(Func<Report, bool> predicate)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.Reports.Where(r => predicate(r)).ToListAsync());
        }
      
        protected async Task<List<EventShard>> FindEventsByAsync(Func<EventLink, bool> predicate)
        {
            List<Guid> guids = await storeSentry.ExecuteReadAsync(ctx => ctx.EventLinks.Where(l => predicate(l)).Select(l => l.OtherId).ToListAsync());

            List<EventShard> events = await storeSentry.ExecuteReadAsync(ctx=> ctx.Events.Where(e => guids.Contains(e.Id)).Select(e => new EventShard
               (
                   e.Id,
                   new UserSilhouette(e.HostId, e.Host.Name),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.IsOpen,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new Character(
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness)
               )).ToListAsync());

            return events;
        }

        protected async Task<int> CountRatingsAsync(Guid id, PostLink.PostLinkType type)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.PostLinks.Where(l => l.OtherId == id && l.Type == type).CountAsync());
        }
    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
