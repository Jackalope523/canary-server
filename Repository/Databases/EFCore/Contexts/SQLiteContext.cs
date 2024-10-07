using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class SQLiteContext : QueryContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseDirectory = Environment.GetEnvironmentVariable("HOME") ?? "C:..\\Repository\\Databases\\EFCore";

            Console.WriteLine("HOME: " + Environment.GetEnvironmentVariable("HOME"));
            optionsBuilder.UseSqlite("Data Source=" + databaseDirectory + "TestDB.db", x => x.UseNetTopologySuite());
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }


    }
}
