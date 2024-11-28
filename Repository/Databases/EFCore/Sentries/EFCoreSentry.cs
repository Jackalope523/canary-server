using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using static Repository.Harbor;

namespace Repository
{
    internal class EFCoreSentry : IDatabaseSentry
    {
        private readonly Func<CanaryContext> initializeContext;

        public EFCoreSentry(Flag flag)
        {
            switch (flag)
            {
                case Flag.Development:
                    initializeContext = () => new DevelopmentContext();
                    break;
                case Flag.Staging:
                    initializeContext = () => new AzureStagingContext();
                    break;
                case Flag.Production:
                    initializeContext = () => new AzureProductionContext();
                    break;
                default:
                    throw new ArgumentException("Invalid Harbor flag: " + nameof(flag));
            }
        }

        public T ExecuteRead<T>(Func<CanaryContext, T> read)
        {
            using (CanaryContext context = initializeContext())
            {
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
        }

        public void ExecuteWrite(Action<CanaryContext> write)
        {
            using (CanaryContext context = initializeContext())
            {
                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        write.Invoke(context);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new DatabaseWriteException(ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public async Task<T> ExecuteReadAsync<T>(Func<CanaryContext, Task<T>> read)
        {
            using (CanaryContext context = initializeContext())
            {
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
        }

        public async Task ExecuteWriteAsync(Action<CanaryContext> write)
        {
            using (CanaryContext context = initializeContext())
            {
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        write.Invoke(context);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new DatabaseWriteException(ex);
                    }
                    finally
                    {
                        await transaction.DisposeAsync();
                        await context.DisposeAsync();
                    }
                }
            }
        }
        public async Task ExecuteWriteAsync(Func<CanaryContext, Task> write)
        {
            using (CanaryContext context = initializeContext())
            {
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await write.Invoke(context);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new DatabaseWriteException(ex);
                    }
                    finally
                    {
                        await transaction.DisposeAsync();
                        await context.DisposeAsync();
                    }
                }
            }
        }

        public Discussion BeginDiscussion()
        {
            return new Discussion(initializeContext());
        }

        public void DiscussWrite(Action<CanaryContext> write, Discussion discussion)
        {
            try
            {
                write.Invoke(discussion.SharedContext);
            }
            catch (Exception ex) 
            {
                discussion.EndNow();
                throw new DatabaseWriteException(ex);
            }
        }

        public void EndDiscussion(Discussion toEnd)
        {
            try
            {
                toEnd.End();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
        }

        public async Task EndDiscussionAsync(Discussion toEnd)
        {
            try
            {
                await toEnd.EndAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
        }
    }
}


