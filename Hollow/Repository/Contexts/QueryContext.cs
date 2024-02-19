using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    public abstract class QueryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserLink> UserLinks { get; set; }
        public DbSet<EventLink> EventLinks { get; set; }
        public DbSet<PostLink> PostLinks { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<EventReport> EventReports { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Penalty> Penalties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.Haunt)
            .HasSrid(4326);

            modelBuilder.Entity<User>().Property(u => u.CurrentLocation)
            .HasSrid(4326);

            modelBuilder.Entity<Event>().Property(e => e.Location)
            .HasSrid(4326);                                     

            modelBuilder.Entity<PostLink>()
               .HasIndex(l => new { l.UserId, l.PostId })
               .IsUnique();

            modelBuilder.Entity<UserReport>()
                .HasOne(r => r.Self)
                .WithMany(u => u.ReporterList);

            modelBuilder.Entity<UserReport>()
                .HasOne(r => r.Other)
                .WithMany(u => u.ReporteeList);

            modelBuilder.Entity<User>()
               .HasMany(u => u.UserLinks)
               .WithOne(l => l.Other);

            modelBuilder.Entity<User>()
                .HasMany(u => u.EventLinks)
                .WithOne(l => l.User);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PostLinks)
                .WithOne(l => l.User);
        }       
    }
}