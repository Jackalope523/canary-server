using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TestContext : QueryContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\ECote\\source\\repos\\project-sparrow\\Hollow\\Repository\\TestDB.db", x => x.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }


    }
}
