namespace Repository
{
    public interface IDatabaseSentry
    {
        public T ExecuteRead<T>(Func<QueryContext, T> read);
        public void ExecuteWrite(Action<QueryContext> write);

        public Task<T> ExecuteReadAsync<T>(Func<QueryContext,Task<T>> read);
        public Task ExecuteWriteAsync(Action<QueryContext> write);
        public Task ExecuteWriteAsync(Func<QueryContext,Task> write);

        public Discussion BeginDiscussion();
        public void DiscussWrite(Action<QueryContext> write, Discussion discussion);
        public void EndDiscussion(Discussion toEnd);
        public Task EndDiscussionAsync(Discussion toEnd);
    }
}