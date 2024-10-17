using Azure.Security.KeyVault.Certificates;
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
            modelBuilder.Entity<User>()
                .HasData(new User()
                {
                    Id = -7,
                    PhoneNumber = "11002003007",
                    Name = "Apple Test Account",
                    IsPhoneConfirmed = true,
                });

            modelBuilder.Entity<User>()
               .HasData(new User()
               {
                   Id = -8,
                   PhoneNumber = "11002003008",
                   Name = "Google Test Account",
                   IsPhoneConfirmed = true,
               });

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
                 .HasMany(u => u.HostedGatherings)
                 .WithOne(g => g.Host)
                 .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                 .HasMany(u => u.InitiatedUserRelationships)
                 .WithOne(l => l.Self)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TargetUserRelationships)
                .WithOne(l => l.Other)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.GatheringLinks)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SnapshotLinks)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReporterList)
                .WithOne(r => r.Self)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReporteeList)
                .WithOne(r => r.Other)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SentTelegrams)
                .WithOne(t => t.Notifier)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedTelegrams)
                .WithOne(t => t.Recipient)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SnapshotReports)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
               .HasMany(u => u.GatheringReports)
               .WithOne(r => r.User)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Snapshots)
               .WithOne(r => r.Owner)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Subscriptions)
               .WithOne(s => s.User)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
             .HasMany(u => u.Feedback)
             .WithOne(f => f.User)
             .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
             .HasMany(u => u.Penalties)
             .WithOne(p => p.Penalized)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
             .HasMany(u => u.GuestClearances)
             .WithOne(c => c.User)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
            .HasMany(u => u.BannerLinks)
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
               .HasMany(g => g.GatheringLink)
               .WithOne(l => l.Gathering)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gathering>()
              .HasMany(g => g.Snapshots)
              .WithOne(s => s.Gathering)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gathering>()
              .HasMany(g => g.GatheringReports)
              .WithOne(l => l.Gathering)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gathering>()
             .HasMany(g => g.GuestClearances)
             .WithOne(c => c.Gathering)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gathering>()
             .HasMany(g => g.UserReports)
             .WithOne(r => r.Gathering)
             .OnDelete(DeleteBehavior.Restrict);

            // Telegram
            modelBuilder.Entity<Telegram>().Property(n => n.Action)
               .HasMaxLength(500);

            // Subscription
            modelBuilder.Entity<Subscription>().Property(s => s.DeviceToken)
              .HasMaxLength(500);

            // User Report
            modelBuilder.Entity<UserReport>().Property(r => r.Notes)
              .HasMaxLength(2000);

            // Gathering Report
            modelBuilder.Entity<GatheringReport>().Property(r => r.Notes)
              .HasMaxLength(2000);

            // Snapshot Report
            modelBuilder.Entity<SnapshotReport>().Property(r => r.Notes)
              .HasMaxLength(2000);

            // Snapshot
            modelBuilder.Entity<Snapshot>()
               .HasMany(s => s.Reports)
               .WithOne(r => r.Snapshot)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Snapshot>()
              .HasMany(s => s.SnapshotLinks)
              .WithOne(l => l.Snapshot)
              .OnDelete(DeleteBehavior.Cascade);

            // Banner
            modelBuilder.Entity<Banner>().Property(b => b.Name)
             .HasMaxLength(100);

            modelBuilder.Entity<Banner>().Property(b => b.Description)
             .HasMaxLength(1000);

            modelBuilder.Entity<Banner>().Property(b => b.Code)
             .HasMaxLength(50);

            modelBuilder.Entity<Banner>().Property(b => b.Color)
            .HasMaxLength(7);

            modelBuilder.Entity<Banner>()
              .HasMany(b => b.Links)
              .WithOne(l => l.Banner)
              .OnDelete(DeleteBehavior.Cascade);

            // Feedback
            modelBuilder.Entity<Feedback>().Property(f => f.Comments)
            .HasMaxLength(300);

            // Snapshot Link
            modelBuilder.Entity<SnapshotLink>()
               .HasIndex(l => new { l.UserId, l.SnapshotId })
               .IsUnique();;

            // User Relationship
            modelBuilder.Entity<UserRelationship>()
               .HasIndex(l => new { l.SelfId, l.OtherId })
               .IsUnique();

            // Gathering Link

            // Banner Link

            // Clearance Link
            modelBuilder.Entity<GuestClearance>()
               .HasIndex(l => new { l.UserId, l.GatheringId})
               .IsUnique();

            // Penalty
        }
    }
}