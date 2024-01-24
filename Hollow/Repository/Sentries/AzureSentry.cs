using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository
{
    public class AzureSentry : Sentry
    {
        public AzureSentry()
        {

        }

        protected override void RefreshContext()
        {
            context = new AzureContext();
        }

        public override T ExecuteRead<T>(Func<QueryContext, T> read)
        {
            RefreshContext();
            try
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return read.Invoke(context);
            }
            catch (Exception ex)
            {
                throw new DatabaseReadException(ex);
            }
            finally
            {
                context.Dispose();
            }
        }
        public override void DiscussWrite(Action<QueryContext> write)
        {
            try
            {
                if (!activeDiscussion)
                {
                    activeDiscussion = true;
                    RefreshContext();
                }
                write.Invoke(context);
            }
            catch (Exception ex)
            {
                context.DisposeAsync();
                activeDiscussion = false;
                throw new DatabaseWriteException(ex);
            }
        }
        public override void ExecuteWrite()
        {
            try
            {
                context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
            finally
            {
                context.Dispose();
                activeDiscussion = false;
            }
        }
        public override void ExecuteWrite(Action<QueryContext> write)
        {
            RefreshContext();
            try
            {
                write.Invoke(context);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
            finally
            {
                context.Dispose();
            }
        }

        public async override Task<T> ExecuteReadAsync<T>(Func<QueryContext, Task<T>> read)
        {
            RefreshContext();
            try
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return await read.Invoke(context);
            }
            catch (Exception ex)
            {
                throw new DatabaseReadException(ex);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }
        public async override Task ExecuteWriteAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
            finally
            {
                await context.DisposeAsync();
                activeDiscussion = false;
            }
        }
        public async override Task ExecuteWriteAsync(Action<QueryContext> write)
        {
            RefreshContext();
            try
            {
                write.Invoke(context);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }
        public async override Task ExecuteWriteAsync(Func<QueryContext,Task> write)
        {
            RefreshContext();
            try
            {
                await write.Invoke(context);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }
    }
}
