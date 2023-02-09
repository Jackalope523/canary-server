using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace DataAccess
{
    public class QueryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }  
        public DbSet<Link> Links { get; set; }
        public DbSet<UserLink> UserLinks { get; set; }
        public DbSet<EventLink> EventLinks { get; set; }  

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test", x => x.UseNetTopologySuite());
         }

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


            SeedData(modelBuilder);
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
                    NormalisedEmail = "",
                    Name = "Signy of Sváfnir",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "1",
                    Email = "",
                    NormalisedEmail = "",
                    Name = "Huginn",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "2",
                    Email = "",
                    NormalisedEmail = "",
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
                { Id = Guid.NewGuid(), SelfId = users[1].Id, OtherId = users[0].Id, Type = UserLink.UserLinkType.Following },
                new UserLink // Muninn follows Signy
                { Id = Guid.NewGuid(), SelfId = users[2].Id, OtherId = users[0].Id, Type = UserLink.UserLinkType.Following  },
                new UserLink // Signy follows Huginn
                { Id = Guid.NewGuid(), SelfId = users[0].Id, OtherId = users[1].Id, Type = UserLink.UserLinkType.Following },
                new UserLink // Signy blocks Muninn
                { Id = Guid.NewGuid(), SelfId = users[0].Id, OtherId = users[2].Id, Type = UserLink.UserLinkType.Blocked }
            };

            List<Event> events = new()
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    HostId = users[1].Id,
                    Name = "The First Few",
                    Description = "nothing interesting",
                    EventType = "campfire,stories",
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
                    EventType = "skiing,drinks,rager",
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
                    EventType = "chill,drinks",
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
                { Id = Guid.NewGuid(), SelfId = users[0].Id, EventId = events[2].Id, Type = EventLink.EventLinkType.Watching },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[1].Id, EventId = events[2].Id, Type = EventLink.EventLinkType.Watching },
                new EventLink
                { Id = Guid.NewGuid(), SelfId = users[2].Id, EventId = events[2].Id, Type = EventLink.EventLinkType.Watching },
            };


            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<UserLink>().HasData(userLinks);
            modelBuilder.Entity<Event>().HasData(events);
            modelBuilder.Entity<EventLink>().HasData(eventLinks);
        }
    }
}