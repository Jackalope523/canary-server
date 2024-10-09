using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    internal abstract class CanaryContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Gathering> Gatherings { get; set; }
        internal DbSet<UserRelationship> UserLinks { get; set; }
        internal DbSet<GatheringLink> GatheringLinks { get; set; }
        internal DbSet<SnapshotLink> SnapshotLinks { get; set; }
        internal DbSet<UserReport> UserReports { get; set; }
        internal DbSet<GatheringReport> GatheringReports { get; set; }
        internal DbSet<SnapshotReport> SnapshotReports { get; set; }
        internal DbSet<Snapshot> Snapshots { get; set; }
        internal DbSet<Telegram> Telegrams { get; set; }
        internal DbSet<Subscription> Subscriptions { get; set; }
        internal DbSet<Penalty> Penalties { get; set; }
        internal DbSet<Banner> Banners { get; set; }
        internal DbSet<BannerLink> BannerLinks { get; set; }
        internal DbSet<GuestClearance> GuestClearances { get; set; }
        internal DbSet<Feedback> Feedback { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>().Property(u => u.Email)
                .HasMaxLength(255);

            modelBuilder.Entity<User>().Property(u => u.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<User>().Property(u => u.Pseudonym)
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
              .WithOne(l => l.Other)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.GatheringLinks)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PostLinks)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Cascade);

            // Gathering
            modelBuilder.Entity<Gathering>().Property(g => g.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Gathering>().Property(g => g.FriendlyLocation)
                .HasMaxLength(255);

            modelBuilder.Entity<Gathering>().Property(g => g.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Gathering>().Property(g => g.Location)
                .HasSrid(4326);

            modelBuilder.Entity<Gathering>()
               .HasMany(g => g.Links)
               .WithOne(l => l.Gathering)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gathering>()
              .HasMany(g => g.Snapshots)
              .WithOne(s => s.Gathering)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gathering>()
              .HasMany(g => g.Reports)
              .WithOne(l => l.Gathering)
              .OnDelete(DeleteBehavior.Cascade);

            // Telegram
            modelBuilder.Entity<Telegram>().Property(n => n.Action)
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

            // Snapshot Report
            modelBuilder.Entity<SnapshotReport>().Property(r => r.Notes)
              .HasMaxLength(2000);

            // Snapshot


            // Banner
            modelBuilder.Entity<Banner>().Property(b => b.Name)
             .HasMaxLength(100);

            modelBuilder.Entity<Banner>().Property(b => b.Description)
             .HasMaxLength(1000);

            modelBuilder.Entity<Banner>().Property(b => b.Code)
             .HasMaxLength(50);

            modelBuilder.Entity<Banner>().Property(b => b.Color)
            .HasMaxLength(7);

            // Feedback
            modelBuilder.Entity<Feedback>().Property(f => f.Comments)
            .HasMaxLength(300);

            // Snapshot Link
            modelBuilder.Entity<SnapshotLink>()
               .HasIndex(l => new { l.UserId, l.SnapshotId })
               .IsUnique();

            // User Link
            modelBuilder.Entity<UserRelationship>()
               .HasIndex(l => new { l.SelfId, l.OtherId })
               .IsUnique();

            // Clearance Link
            modelBuilder.Entity<GuestClearance>()
               .HasIndex(l => new { l.UserId, l.GatheringId})
               .IsUnique();
        }
    }
}