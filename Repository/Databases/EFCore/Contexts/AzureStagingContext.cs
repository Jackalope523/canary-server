using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class AzureStagingContext : CanaryContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=tcp:sparrow-stores.database.windows.net,1433;Initial Catalog=LaboratoryV2;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
            optionsBuilder.UseSqlServer(connectionString, x => x.
                UseNetTopologySuite().
                MigrationsHistoryTable("__StagingMigrationsHistory").
                EnableRetryOnFailure());

            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*
             modelBuilder.HasSequence<long>("UserIds")
                 .StartsAt(100)
                 .IncrementsBy(1);

             modelBuilder.Entity<User>().Property(u => u.Id)
                 .HasDefaultValue("NEXT VALUE FOR UserIds");
             */
        }
    }
}
