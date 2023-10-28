using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class AzureContext : QueryContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:sparrow-stores.database.windows.net,1433;Initial Catalog=Laboratory;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";", x => x.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
