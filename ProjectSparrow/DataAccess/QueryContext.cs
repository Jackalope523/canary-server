using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
using Microsoft.Extensions.Options;

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

            modelBuilder.Entity<UserLink>()
                .Property(l => l.Type)
                .HasColumnName("Type");


            modelBuilder.Entity<EventLink>()
                .Property(l => l.SelfId)
                .HasColumnName("SelfId");

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
                    Passkey = "",
                    Name = "Signy of Sváfnir",
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "1",
                    Passkey = "",
                    Name = "Huginn",
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "2",
                    Passkey = "",
                    Name = "Muninn",
                }
            };

            modelBuilder.Entity<User>().HasData(users);

            List<UserLink> links = new()
            {
                new UserLink
                {
                    Id = Guid.NewGuid(),
                    SelfId = users[1].Id,
                    OtherId = users[0].Id,
                    Type = UserLink.UserLinkType.Following
                },
                new UserLink
                {
                    Id = Guid.NewGuid(),
                    SelfId = users[2].Id,
                    OtherId = users[0].Id,
                    Type = UserLink.UserLinkType.Following
                },
                new UserLink
                {
                    Id = Guid.NewGuid(),
                    SelfId = users[0].Id,
                    OtherId = users[1].Id,
                    Type = UserLink.UserLinkType.Following
                },
                new UserLink
                {
                    Id = Guid.NewGuid(),
                    SelfId = users[0].Id,
                    OtherId = users[2].Id,
                    Type = UserLink.UserLinkType.Blocked
                }
            };

            modelBuilder.Entity<UserLink>().HasData(links);
        }
    }
}