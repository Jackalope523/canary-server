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
            context.SaveChangesAsync();
            context.DisposeAsync();
            activeDiscussion = false;
        }
        public override void ExecuteWrite(Action<QueryContext> write)
        {         
            using (context = new TestContext())
            {
                write.Invoke(context);
                context.SaveChanges();
            }
        }      
                          
        public async override Task<T> ExecuteReadAsync<T>(Func<QueryContext, Task<T>> read)
        {
            using (context = new TestContext())
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return await read.Invoke(context);
            }
        }  
        public async override Task ExecuteWriteAsync()
        {
            await context.SaveChangesAsync();
            await context.DisposeAsync();
            activeDiscussion = false;
        }
        public async override Task ExecuteWriteAsync(Action<QueryContext> write)
        {
            using (context = new TestContext())
            {
                write.Invoke(context);
                await context.SaveChangesAsync();
            }
        }
    }
}
