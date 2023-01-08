using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;

namespace DataAccess
{
    public class QueryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }  
        public DbSet<Link> Links { get; set; }
        public DbSet<UserLink> UsersLinks { get; set; }
        public DbSet<EventLink> EventLinks { get; set; }  

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