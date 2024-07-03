using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    internal abstract class QueryContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Gathering> Gatherings { get; set; }
        internal DbSet<UserLink> UserLinks { get; set; }
        internal DbSet<GatheringLink> GatheringLinks { get; set; }
        internal DbSet<SnapshotLink> SnapshotLinks { get; set; }
        internal DbSet<UserReport> UserReports { get; set; }
        internal DbSet<GatheringReport> GatheringReports { get; set; }
        internal DbSet<Snapshot> Snapshots { get; set; }
        internal DbSet<Note> Notes { get; set; }
        internal DbSet<Subscription> Subscriptions { get; set; }
        internal DbSet<Penalty> Penalties { get; set; }
        internal DbSet<Banner> Banners { get; set; }
        internal DbSet<BannerLink> BannerLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>().Property(u => u.Email)
                .HasMaxLength(255);

            modelBuilder.Entity<User>().Property(u => u.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<User>().Property(u => u.NormalisedEmail)
                .HasMaxLength(255);

            modelBuilder.Entity<User>().Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            modelBuilder.Entity<User>().Property(u => u.SecurityStamp)
                .HasMaxLength(50);

            modelBuilder.Entity<User>().Property(u => u.Haunt)
                .HasSrid(4326);

            modelBuilder.Entity<User>().Property(u => u.CurrentLocation)
                .HasSrid(4326);

            modelBuilder.Entity<Gathering>().Property(e => e.Location)
                .HasSrid(4326);

            modelBuilder.Entity<User>()
              .HasMany(u => u.UserLinks)
              .WithOne(l => l.Other);

            modelBuilder.Entity<User>()
                .HasMany(u => u.GatheringLinks)
                .WithOne(l => l.User);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PostLinks)
                .WithOne(l => l.User);

            // Gathering
            modelBuilder.Entity<Gathering>().Property(g => g.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Gathering>().Property(g => g.FriendlyLocation)
                .HasMaxLength(255);

            modelBuilder.Entity<Gathering>().Property(g => g.HeroImageURL)
                .HasMaxLength(2083); // URL length limit

            modelBuilder.Entity<Gathering>().Property(g => g.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Gathering>().Property(g => g.Location)
                .HasSrid(4326);

            // Note
            modelBuilder.Entity<Note>().Property(n => n.Message)
               .HasMaxLength(5000);

            modelBuilder.Entity<Note>().Property(n => n.Action)
               .HasMaxLength(500);

            // Subscription
            modelBuilder.Entity<Subscription>().Property(s => s.DeviceToken)
              .HasMaxLength(500);

            // User Report
            modelBuilder.Entity<UserReport>().Property(r => r.Notes)
              .HasMaxLength(2000);

            modelBuilder.Entity<UserReport>()
               .HasOne(r => r.Self)
               .WithMany(u => u.ReporterList);

            modelBuilder.Entity<UserReport>()
                .HasOne(r => r.Other)
                .WithMany(u => u.ReporteeList);

            // Gathering Report
            modelBuilder.Entity<GatheringReport>().Property(r => r.Notes)
              .HasMaxLength(2000);

            // Snapshot
            modelBuilder.Entity<Snapshot>().Property(s => s.PhotoURL)
             .HasMaxLength(2083); // URL length limit

            // Banner
            modelBuilder.Entity<Banner>().Property(b => b.Name)
             .HasMaxLength(100);

            modelBuilder.Entity<Banner>().Property(b => b.Description)
             .HasMaxLength(1000);

            // Snapshot Link
            modelBuilder.Entity<SnapshotLink>()
               .HasIndex(l => new { l.UserId, l.PostId })
               .IsUnique();
        }
    }
}