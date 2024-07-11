using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class SQLiteContext : QueryContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\ECote\\source\\repos\\project-hollow\\Repository\\Databases\\EFCore\\TestDB.db", x => x.UseNetTopologySuite());
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }


    }
}
