using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    internal abstract class QueryContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Event> Events { get; set; }
        internal DbSet<UserLink> UserLinks { get; set; }
        internal DbSet<EventLink> EventLinks { get; set; }
        internal DbSet<PostLink> PostLinks { get; set; }
        internal DbSet<UserReport> UserReports { get; set; }
        internal DbSet<EventReport> EventReports { get; set; }
        internal DbSet<Post> Posts { get; set; }
        internal DbSet<Note> Notes { get; set; }
        internal DbSet<Subscription> Subscriptions { get; set; }
        internal DbSet<Entities.Penalty> Penalties { get; set; }
        //internal DbSet<Banner> Banners { get; set; }
        //internal DbSet<BannerLink> BannerLinks { get; set; }

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