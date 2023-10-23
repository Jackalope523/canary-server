using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Contexts
{
    internal class AzureContext : QueryContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlCore("Core=tcp:sparrow-stores.database.windows.net,1433;Initial Catalog=Laboratory;Encrypt=True;TrustCoreCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";", x => x.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
