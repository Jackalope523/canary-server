using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using static Repository.Harbor;

namespace Repository
{
    internal class AzureStagingContext : CanaryContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=tcp:sparrow-stores.database.windows.net,1433;Initial Catalog=LaboratoryV2;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
            optionsBuilder.UseSqlServer(connectionString, x => x.UseNetTopologySuite().MigrationsHistoryTable("__StagingMigrationsHistory"));
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
