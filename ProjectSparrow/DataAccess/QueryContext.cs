using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;

namespace DataAccess
{
    public class QueryContext : DbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Event> Events { get; set; }  
        DbSet<Link> Links { get; set; }
        DbSet<UserLink> UsersLinks { get; set; }
        DbSet<EventLink> EventLinks { get; set; }  

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
         }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>()
                .HasDiscriminator<string>("link_type")
                .HasValue<UserLink>("user")
                .HasValue<EventLink>("event");

            modelBuilder.Entity<Link>()
                .Property<string>("Discriminator")
                .HasMaxLength(50);

            modelBuilder.Entity<UserLink>()
                .Property(l => l.SelfId)
                .HasColumnName("SelfId");


            modelBuilder.Entity<EventLink>()
                .Property(l => l.SelfId)
                .HasColumnName("SelfId");

            modelBuilder.Entity<Link>()
                .HasOne(a => a.Self)
                .WithMany(b => b.Links);

            modelBuilder.Entity<EventLink>()
                .HasOne(a => a.Event)
                .WithMany(b => b.Links);
        }
    }
}