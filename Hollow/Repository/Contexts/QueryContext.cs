using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    public abstract class QueryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<UserLink> UserLinks { get; set; }
        public DbSet<EventLink> EventLinks { get; set; }
        public DbSet<PostLink> PostLinks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<EventReport> EventReports { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Penalty> Penalties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.AccountStatus)
                .HasColumnName("AccountStatus");

            modelBuilder.Entity<Link>()
                .HasDiscriminator<string>("link_type")
                .HasValue<UserLink>("user")
                .HasValue<EventLink>("event")
                .HasValue<PostLink>("post");

            modelBuilder.Entity<UserLink>()
                .Property(l => l.SelfId)
                .HasColumnName("SelfId");

            modelBuilder.Entity<UserLink>()
                .Property(l => l.Type)
                .HasColumnName("Type");

            modelBuilder.Entity<EventLink>()
                .Property(l => l.SelfId)
                .HasColumnName("SelfId");

            modelBuilder.Entity<EventLink>()
                .Property(l => l.OtherId)
                .HasColumnName("OtherId");

            modelBuilder.Entity<EventLink>()
                .Property(l => l.Type)
                .HasColumnName("Type");

            modelBuilder.Entity<PostLink>()
                .Property(l => l.SelfId)
                .HasColumnName("SelfId");

            modelBuilder.Entity<PostLink>()
                .Property(l => l.Type)
                .HasColumnName("Type");
           
            modelBuilder.Entity<Report>()
                .HasDiscriminator<string>("report_type")
                .HasValue<UserReport>("user")
                .HasValue<EventReport>("event");

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Self)
                .WithMany(u => u.ReporterList);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Other)
                .WithMany(u => u.ReporteeList);

            modelBuilder.Entity<Report>()
               .HasOne(r => r.Event)
               .WithMany(e => e.Reports);

            //SeedData(modelBuilder);
        }       
    }
}