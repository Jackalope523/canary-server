
namespace Repository
{
    public abstract class Sentry
    {
        protected QueryContext context;

        public abstract void DiscussWrite(Action<QueryContext> write);
        public abstract void ExecuteWrite();
        public abstract void ExecuteWrite(Action<QueryContext> write);
        public abstract T ExecuteRead<T>(Func<QueryContext,T> read);
    }
}