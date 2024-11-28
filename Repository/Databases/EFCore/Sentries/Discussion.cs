using Microsoft.EntityFrameworkCore.Storage;

namespace Repository
{
    internal class Discussion
    {
        internal CanaryContext SharedContext { get; private set; }
        internal IDbContextTransaction Transaction { get; private set; }

        internal Discussion(CanaryContext sharedContext)
        {
            SharedContext = sharedContext;
            Transaction = sharedContext.Database.BeginTransaction();
        }

        internal void End()
        {
            try
            {
                SharedContext.SaveChanges();
                Transaction.Commit();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                throw;
            }
            finally
            {
                Transaction.Dispose();
                SharedContext.Dispose();
            }                
        }

        internal async Task  EndAsync()
        {
            try
            {
                await SharedContext.SaveChangesAsync();
                await Transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await Transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Transaction.DisposeAsync();
                await SharedContext.DisposeAsync();
            }
        }

        internal void EndNow()
        {
            Transaction.Rollback();
            Transaction.Dispose();
            SharedContext.Dispose();
        }
    }
}
