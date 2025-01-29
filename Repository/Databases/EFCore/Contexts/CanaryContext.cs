using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    internal abstract class CanaryContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Gathering> Gatherings { get; set; }
        internal DbSet<UserRelationship> UserRelationships { get; set; }
        internal DbSet<GatheringLink> GatheringLinks { get; set; }
        internal DbSet<SnapshotLink> SnapshotLinks { get; set; }
        internal DbSet<UserReport> UserReports { get; set; }
        internal DbSet<GatheringReport> GatheringReports { get; set; }
        internal DbSet<SnapshotReport> SnapshotReports { get; set; }
        internal DbSet<Snapshot> Snapshots { get; set; }
        internal DbSet<Telegram> Telegrams { get; set; }
        internal DbSet<Subscription> Subscriptions { get; set; }
        internal DbSet<Penalty> Penalties { get; set; }
        internal DbSet<GuestClearance> GuestClearances { get; set; }
        internal DbSet<Feedback> Feedback { get; set; }
        internal DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity
            modelBuilder.Ignore<Entity>();

            // User
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.SoftDeleted);

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

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.CompanionshipCode)
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.NormalisedEmail)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(u => u.SecurityStamp)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Haunt)
                .HasSrid(4326);

            modelBuilder.Entity<User>()
                .Property(u => u.CurrentLocation)
                .HasSrid(4326);

            modelBuilder.Entity<User>()
                .Property(u => u.SocialInvitations)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.CompanionActivity)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.GatheringReminders)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.GatheringActivity)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.GatheringDiscovery)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .HasMany(u => u.HostedGatherings)
                .WithOne(g => g.Host)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.InitiatedUserRelationships)
                .WithOne(l => l.Self)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TargetUserRelationships)
                .WithOne(l => l.Other)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.GatheringLinks)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SnapshotLinks)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReporterList)
                .WithOne(r => r.Self)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReporteeList)
                .WithOne(r => r.Other)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SentTelegrams)
                .WithOne(t => t.Notifier)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedTelegrams)
                .WithOne(t => t.Recipient)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SnapshotReports)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.GatheringReports)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Snapshots)
                .WithOne(r => r.Owner)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Feedback)
                .WithOne(f => f.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Penalties)
                .WithOne(p => p.Penalized)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.GuestClearances)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Notifications)
               .WithOne(n => n.Recipient)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Snapshots)
               .WithOne(r => r.Owner)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Subscriptions)
               .WithOne(s => s.User)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
             .HasMany(u => u.Feedback)
             .WithOne(f => f.User)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
             .HasMany(u => u.Penalties)
             .WithOne(p => p.Penalized)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
             .HasMany(u => u.GuestClearances)
             .WithOne(c => c.User)
             .OnDelete(DeleteBehavior.Restrict);

            // Gathering
            modelBuilder.Entity<Gathering>()
                .HasQueryFilter(g => !g.SoftDeleted);

            modelBuilder.Entity<Gathering>()
                .Property(g => g.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Gathering>()
                .Property(g => g.FriendlyLocation)
                .HasMaxLength(255);

            modelBuilder.Entity<Gathering>()
                .Property(g => g.Title)
                .HasMaxLength(100);

            modelBuilder.Entity<Gathering>()
                .Property(g => g.Location)
                .HasSrid(4326);

            modelBuilder.Entity<Gathering>()
                .HasMany(g => g.GatheringLink)
                .WithOne(l => l.Gathering)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gathering>()
                .HasMany(g => g.Snapshots)
                .WithOne(s => s.Gathering)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gathering>()
                .HasMany(g => g.GatheringReports)
                .WithOne(l => l.Gathering)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gathering>()
                .HasMany(g => g.GuestClearances)
                .WithOne(c => c.Gathering)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gathering>()
                .HasMany(g => g.UserReports)
                .WithOne(r => r.Gathering)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gathering>()
               .HasMany(u => u.Notifications)
               .WithOne(n => n.Gathering)
               .OnDelete(DeleteBehavior.Restrict);

            // Telegram
            modelBuilder.Entity<Telegram>()
                .HasQueryFilter(t => !t.SoftDeleted);
                
            modelBuilder.Entity<Telegram>()
                .Property(n => n.Action)
                .HasMaxLength(500);

            // Subscription
            modelBuilder.Entity<Subscription>()
                .HasQueryFilter(s => !s.SoftDeleted);

            modelBuilder.Entity<Subscription>()
                .Property(s => s.DeviceToken)
                .HasMaxLength(500);

            // User Report
            modelBuilder.Entity<UserReport>()
                .HasQueryFilter(r => !r.SoftDeleted);

            modelBuilder.Entity<UserReport>()
                .Property(r => r.Notes)
                .HasMaxLength(2000);

            // Gathering Report
            modelBuilder.Entity<GatheringReport>()
                .HasQueryFilter(r => !r.SoftDeleted);

            modelBuilder.Entity<GatheringReport>()
                .Property(r => r.Notes)
                .HasMaxLength(2000);

            // Snapshot Report
            modelBuilder.Entity<SnapshotReport>()
                .HasQueryFilter(r => !r.SoftDeleted);

            modelBuilder.Entity<SnapshotReport>()
                .Property(r => r.Notes)
                .HasMaxLength(2000);

            // Snapshot
            modelBuilder.Entity<Snapshot>()
                .HasQueryFilter(s => !s.SoftDeleted);

            modelBuilder.Entity<Snapshot>()
                .HasMany(s => s.Reports)
                .WithOne(r => r.Snapshot)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Snapshot>()
                .HasMany(s => s.SnapshotLinks)
                .WithOne(l => l.Snapshot)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback
            modelBuilder.Entity<Feedback>()
                .HasQueryFilter(f => !f.SoftDeleted);

            modelBuilder.Entity<Feedback>().Property(f => f.Comments)
                .HasMaxLength(300);

            // Snapshot Link
            modelBuilder.Entity<SnapshotLink>()
                .HasQueryFilter(l => !l.SoftDeleted);

            modelBuilder.Entity<SnapshotLink>()
                .HasIndex(l => new { l.UserId, l.SnapshotId })
                .IsUnique();;

            // User Relationship
            modelBuilder.Entity<UserRelationship>()
                .HasQueryFilter(r => !r.SoftDeleted);

            modelBuilder.Entity<UserRelationship>()
                .HasIndex(l => new { l.SelfId, l.OtherId })
                .IsUnique();

            // Gathering Link
            modelBuilder.Entity<GatheringLink>()
                .HasQueryFilter(l => !l.SoftDeleted);

            // Guest Clearance
            modelBuilder.Entity<GuestClearance>()
                .HasQueryFilter(c => !c.SoftDeleted);

            modelBuilder.Entity<GuestClearance>()
                .HasIndex(l => new { l.UserId, l.GatheringId})
                .IsUnique();

            // Penalty
            modelBuilder.Entity<Penalty>()
                .HasQueryFilter(p => !p.SoftDeleted);

            // Notifications
            modelBuilder.Entity<Notification>()
                .HasQueryFilter(n => !n.SoftDeleted);

            modelBuilder.Entity<Notification>()
                .Property(n => n.NotificationId)
                .HasMaxLength(36);
        }
    }
}