using Microsoft.EntityFrameworkCore;

namespace Repository.Contexts
{
    internal class RapidFlushContext : QueryContext
    {
        private int maxTransactions;
        private int flushCounter = 0;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test", x => x.UseNetTopologySuite());
        }

        public RapidFlushContext Execute()
        {

            return this;
        }
    }
}
