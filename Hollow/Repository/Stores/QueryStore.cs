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
    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
