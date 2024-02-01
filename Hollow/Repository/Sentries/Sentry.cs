namespace Repository
{
    public abstract class Sentry
    {
        protected QueryContext context;
        protected bool activeDiscussion = false;

        protected abstract void RefreshContext();
        public abstract T ExecuteRead<T>(Func<QueryContext, T> read);
        public abstract void DiscussWrite(Action<QueryContext> write);
        public abstract void ExecuteWrite();
        public abstract void ExecuteWrite(Action<QueryContext> write);

        public abstract Task<T> ExecuteReadAsync<T>(Func<QueryContext,Task<T>> read);
        public abstract Task ExecuteWriteAsync();
        public abstract Task ExecuteWriteAsync(Action<QueryContext> write);
        public abstract Task ExecuteWriteAsync(Func<QueryContext,Task> write);

    }
}