using Microsoft.EntityFrameworkCore;

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
        protected async Task AddLinkOperationAsync(Link link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Links.Add(link));
        }
        protected async Task RemoveLinkOperationAsync(UserLink link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.UserLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
        }
        protected async Task RemoveLinkOperationAsync(EventLink link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
        }
        protected async Task RemoveLinkOperationAsync(PostLink link)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.PostLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
        }       
    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
