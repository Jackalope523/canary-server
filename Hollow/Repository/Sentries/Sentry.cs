using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public abstract class Sentry
    {
        public abstract T ExecuteRead<T>(Func<QueryContext, T> read);
        public abstract void ExecuteWrite(Action<QueryContext> write);

        public abstract Task<T> ExecuteReadAsync<T>(Func<QueryContext,Task<T>> read);
        public abstract Task ExecuteWriteAsync(Action<QueryContext> write);
        public abstract Task ExecuteWriteAsync(Func<QueryContext,Task> write);

        public abstract Discussion BeginDiscussion();
        public abstract void DiscussWrite(Action<QueryContext> write, Discussion discussion);
        public abstract void EndDiscussion(Discussion toEnd);
        public abstract Task EndDiscussionAsync(Discussion toEnd);


    }
}