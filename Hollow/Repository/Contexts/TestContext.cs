using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TestContext : QueryContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=/Users/coss/Desktop/Projects/project-sparrow/Hollow/Repository/TestDB.db", x => x.UseNetTopologySuite());
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }


    }
}
