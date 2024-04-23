
using System.Runtime.CompilerServices;

namespace Repository
{
    internal class Discussion
    {
        internal QueryContext SharedContext { get; private set; }

        internal Discussion(QueryContext sharedContext)
        {
            SharedContext = sharedContext;
        }

        internal void End()
        {
            try
            {
                SharedContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                SharedContext.Dispose();
            }                
        }

        internal async Task  EndAsync()
        {
            try
            {
                await SharedContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await SharedContext.DisposeAsync();
            }
        }

        internal void EndNow()
        {
            SharedContext.Dispose();
        }
    }
}
