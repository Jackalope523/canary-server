using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TestSentry : Sentry
    {
        private bool activeDiscussion = false;

        public TestSentry() 
        {

        }      

        public override T ExecuteRead<T>(Func<QueryContext, T> read)
        {
            using (context = new TestContext())
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return read.Invoke(context);
            }
        }

        public override void ExecuteWrite(Action<QueryContext> write)
        {
            using (context = new TestContext())
            {
                write.Invoke(context);
                context.SaveChanges();
            }
        }

        public override void DiscussWrite(Action<QueryContext> write)
        {
            if (!activeDiscussion)
            {
                activeDiscussion = true;
                context = new TestContext();
            }
            write.Invoke(context);
        }

        public override void ExecuteWrite()
        {
            context.SaveChanges();
            context.Dispose();
            activeDiscussion = false;
        }
    }
}
