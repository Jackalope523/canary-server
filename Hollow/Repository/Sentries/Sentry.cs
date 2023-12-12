
namespace Repository
{
    public abstract class Sentry
    {
        protected QueryContext context;

        public abstract Task DiscussWriteAsync(Action<QueryContext> write);
        public abstract Task ExecuteWriteAsync();
        public abstract Task ExecuteWriteAsync(Action<QueryContext> write);
        public abstract T ExecuteRead<T>(Func<QueryContext, T> read);
        public abstract Task<T> ExecuteReadAsync<T>(Func<QueryContext,Task<T>> read);
    }
}