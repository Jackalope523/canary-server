
using System.Runtime.CompilerServices;

namespace Repository
{
    public class Discussion
    {
        public QueryContext SharedContext { get; private set; }

        public Discussion(QueryContext sharedContext)
        {
            SharedContext = sharedContext;
        }

        public void End()
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

        public async Task  EndAsync()
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

        public void EndNow()
        {
            SharedContext.Dispose();
        }
    }
}
