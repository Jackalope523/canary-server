using NetTopologySuite.Geometries;
using Server.Boundaries;

namespace Repository.Entities
{
    public class User
    {
        public Guid Id { get; init; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateOfBirth { get; init; }
        public DateTimeOffset JoinDate { get; init; }
        public int Reputation { get; set; }

        public string NormalizedEmail { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public DateTimeOffset? LockoutDate { get; set; }
        public int AccessTries { get; set; }
        public UserAccountStatus AccountStatus { get; set; }

        // Vector
        public int Extroversion { get; init; }
        public int Athleticisme { get; init; }
        public int Openness { get; init; }
        public int Chaos { get; init; }
        public int Competitiveness { get; init; }
        public int Industriousness { get; init; }
        public int NightOwl { get; init; }

        //Geolocation: X = Longitude Y = Latitude
        public Point Haunt { get; set; }
        public double HauntRadius { get; set; }
        public Point CurrentLocation { get; set; }
        public double CurrentRadius { get; set; }


        // Navigation Properties
        internal List<Link> Links { get; set; }
        internal List<Report> ReporterList { get; set; }
        internal List<Report> ReporteeList { get; set; }
        internal List<Post> Posts { get; set; }

        public ThinUser ToThinUser()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed,
                SecurityStamp, LockoutDate, AccessTries, AccountStatus,
                JoinDate, Reputation, -1);
        }
        public ThinnerUser ToThinnerUser()
        {
            return new(Id, Name);
        }
        public ThinProfile ToThinProfile()
        {
            return new(Id, Name, Reputation, 0);
        }
    }
}
