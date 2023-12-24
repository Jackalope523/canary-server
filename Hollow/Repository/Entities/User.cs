using NetTopologySuite.Geometries;
using Core.Boundaries;

namespace Repository
{
    public class User
    {
        public Guid Id { get; init; } = DefaultId;
        public string PhoneNumber { get; set; } = DefaultPhoneNumber;
        public string Email { get; set; } = DefaultEmail;
        public string NormalisedEmail { get; set; } = DefaultNormalisedEmail;
        public string Name { get; set; } = DefaultName;
        public DateTimeOffset DateOfBirth { get; init; } = DefaultDateOfBirth;
        public DateTimeOffset JoinDate { get; init; } = DefaultJoinDate;
        public int Reputation { get; set; } = DefaultReputation;
        public bool IsPhoneConfirmed { get; set; } = DefaultIsPhoneConfirmed;
        public bool IsEmailConfirmed { get; set; } = DefaultIsEmailConfirmed;
        public string SecurityStamp { get; set; } = DefaultSecurityStamp;
        public DateTimeOffset? LockoutDate { get; set; } = DefaultLockoutDate;
        public int AccessTries { get; set; } = DefaultAccessTries;
        public UserAccountStatus AccountStatus { get; set; } = DefaultAccountStatus;

        // Vector
        public int Extroversion { get; init; } = DefaultExtroversion;
        public int Athleticisme { get; init; } = DefaultAthleticisme;
        public int Openness { get; init; } = DefaultOpenness;
        public int Chaos { get; init; } = DefaultChaos;
        public int Competitiveness { get; init; } = DefaultCompetitiveness;
        public int Industriousness { get; init; } = DefaultIndustriousness;
        public int NightOwl { get; init; } = DefaultNightOwl;

        //Geolocation: X = Longitude Y = Latitude
        public Point Haunt { get; set; } = DefaultHaunt;
        public double HauntRadius { get; set; } = DefaultHauntRadius;
        public int HauntWheight { get; set; } = DefaultHauntWheight;
        public Point CurrentLocation { get; set; } = DefaultCurrentLocation;
        public double CurrentRadius { get; set; } = DefaultCurrentRadius;

        // Navigation Properties
        internal List<Link> Links { get; set; }
        internal List<Report> ReporterList { get; set; }
        internal List<Report> ReporteeList { get; set; }
        internal List<Post> Posts { get; set; }

        // Default Values
        public static Guid DefaultId { get; set; } = Guid.Empty;
        public static string DefaultPhoneNumber { get; set; } = "000-000-0000";
        public static string DefaultEmail { get; set; } = "JohnDoe@Test.com";
        public static string DefaultNormalisedEmail { get; set; } = "johndoe@test.com";
        public static string DefaultName { get; set; } = "John Doe";
        public static DateTimeOffset DefaultDateOfBirth { get; set; } = DateTimeOffset.MinValue;
        public static DateTimeOffset DefaultJoinDate { get; set; } = DateTimeOffset.MinValue;
        public static int DefaultReputation { get; set; } = 50;
        public static bool DefaultIsPhoneConfirmed { get; set; } = false;
        public static bool DefaultIsEmailConfirmed { get; set; } = false;
        public static string DefaultSecurityStamp { get; set; } = "ijhbzdfsoiuh9ui239";
        public static DateTimeOffset? DefaultLockoutDate { get; set; } = DateTimeOffset.MaxValue;
        public static int DefaultAccessTries { get; set; } = 3;
        public static UserAccountStatus DefaultAccountStatus { get; set; } = UserAccountStatus.active;

        // Vector
        public static int DefaultExtroversion { get; set; } = 50;
        public static int DefaultAthleticisme { get; set; } = 50;
        public static int DefaultOpenness { get; set; } = 50;
        public static int DefaultChaos { get; set; } = 50;
        public static int DefaultCompetitiveness { get; set; } = 50;
        public static int DefaultIndustriousness { get; set; } = 50;
        public static int DefaultNightOwl { get; set; } = 50;

        //Geolocation: X = Longitude Y = Latitude
        public static Point DefaultHaunt { get; set; } = new Point(40.7128, -74.0060);
        public static double DefaultHauntRadius { get; set; } = 10;
        public static int DefaultHauntWheight { get; set; } = 0;
        public static Point DefaultCurrentLocation { get; set; } = new Point(40.7128, -74.0060);
        public static double DefaultCurrentRadius { get; set; } = 10;
    }
}
