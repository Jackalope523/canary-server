using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TestSentry : Sentry
    {
        public TestSentry() 
        {
            context = new TestContext();
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
                
            }
        }

        public override void DiscussWrite(Action<QueryContext> write)
        {
            if (context == null) context = new TestContext();
            write.Invoke(context);
            
        }

        public override void ExecuteWrite()
        {
            context.SaveChanges();
            context.Dispose();
        }
    }
}
