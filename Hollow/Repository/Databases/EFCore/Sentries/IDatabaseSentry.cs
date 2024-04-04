namespace Repository
{
    internal interface IDatabaseSentry
    {
        internal T ExecuteRead<T>(Func<QueryContext, T> read);
        internal void ExecuteWrite(Action<QueryContext> write);

        internal Task<T> ExecuteReadAsync<T>(Func<QueryContext,Task<T>> read);
        internal Task ExecuteWriteAsync(Action<QueryContext> write);
        internal Task ExecuteWriteAsync(Func<QueryContext,Task> write);

        internal Discussion BeginDiscussion();
        internal void DiscussWrite(Action<QueryContext> write, Discussion discussion);
        internal void EndDiscussion(Discussion toEnd);
        internal Task EndDiscussionAsync(Discussion toEnd);
    }
}