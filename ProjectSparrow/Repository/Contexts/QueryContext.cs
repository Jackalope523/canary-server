using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Repository.Entities;

namespace Repository.Contexts
{
    public abstract class QueryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<UserLink> UserLinks { get; set; }
        public DbSet<EventLink> EventLinks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.AccountStatus)
                .HasColumnName("AccountStatus");


            modelBuilder.Entity<Link>()
                .HasDiscriminator<string>("link_type")
                .HasValue<UserLink>("user")
                .HasValue<EventLink>("event");


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
                .Property(l => l.Type)
                .HasColumnName("Type");


            modelBuilder.Entity<Link>()
                .HasOne(a => a.Self)
                .WithMany(b => b.Links);

            modelBuilder.Entity<EventLink>()
                .HasOne(a => a.Event)
                .WithMany(b => b.Links);

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

        private void SeedData(ModelBuilder modelBuilder)
        {
            List<User> users = new()
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "0",
                    Email = "",
                    NormalizedEmail = "",
                    Name = "Signy of Sváfnir",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "1",
                    Email = "",
                    NormalizedEmail = "",
                    Name = "Huginn",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "2",
                    Email = "",
                    NormalizedEmail = "",
                    Name = "Muninn",
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };

            List<UserLink> userLinks = new()
            {
                // Signy <---> Huginn
                //    ^
                //    |
                //    X
                // Muninn
                new UserLink // Huginn follows Signy
                { Id = Guid.NewGuid(), SelfId = users[1].Id, OtherId = users[0].Id, Type = UserLink.UserLinkType.Follow },
                new UserLink // Muninn follows Signy
                { Id = Guid.NewGuid(), SelfId = users[2].Id, OtherId = users[0].Id, Type = UserLink.UserLinkType.Follow },
                new UserLink // Signy follows Huginn
                { Id = Guid.NewGuid(), SelfId = users[0].Id, OtherId = users[1].Id, Type = UserLink.UserLinkType.Follow },
                new UserLink // Signy blocks Muninn
                { Id = Guid.NewGuid(), SelfId = users[0].Id, OtherId = users[2].Id, Type = UserLink.UserLinkType.Block }
            };

            List<Event> events = new()
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    HostId = users[1].Id,
                    Name = "The First Few",
                    Description = "nothing interesting",
                    Type = "campfire,stories",
                    Location = new Point(0, 0) { SRID=4237 },
                    StartTime = new DateTimeOffset(800, 4, 2, 18, 00, 0, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(800, 4, 3, 1, 37, 0, TimeSpan.Zero)
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    HostId = users[1].Id,
                    Name = "Then There Were Two",
                    Description = "still nothing interesting",
                    Type = "skiing,drinks,rager",
                    Location = new Point(0, 0) { SRID=4237 },
                    StartTime = new DateTimeOffset(800, 11, 2, 13, 00, 0, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(800, 11, 4, 11, 03, 0, TimeSpan.Zero)
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    HostId = users[0].Id,
                    Name = "Masquerade",
                    Description = "something interesting",
                    Type = "chill,drinks",
                    Location = new Point(23.4413325,-76.0092066) { SRID=4237 },
                    StartTime = new DateTimeOffset(2025, 6, 25, 17, 00, 0, TimeSpan.Zero)
                }
            };

            List<EventLink> eventLinks = new()
            {
                // The First Few Attendees
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[0].Id, EventId = events[0].Id },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[1].Id, EventId = events[0].Id },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[2].Id, EventId = events[0].Id },

                // Then There Were Two Attendees
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[0].Id, EventId = events[1].Id },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[1].Id, EventId = events[1].Id },

                // Masquerade Attendees
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[0].Id, EventId = events[2].Id, Type = EventLink.EventLinkType.Watch },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[1].Id, EventId = events[2].Id, Type = EventLink.EventLinkType.Watch },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[2].Id, EventId = events[2].Id, Type = EventLink.EventLinkType.Watch },
            };


            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<UserLink>().HasData(userLinks);
            modelBuilder.Entity<Event>().HasData(events);
            modelBuilder.Entity<EventLink>().HasData(eventLinks);
        }
    }
}