using NetTopologySuite.Geometries;
using Core.Boundaries;
using Repository.Entities;

namespace Repository
{
    public class User
    {
        public ulong Id { get; init; } = DefaultId;
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
        public ulong? CurrentEvent { get; set; } = DefaultCurrentEvent;
        public bool PendingDeletion { get; set; } = DefaultPendingDeletion;

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
        public List<UserLink>? UserLinks { get; set; }
        public List<EventLink>? EventLinks { get; set; }
        public List<PostLink>? PostLinks { get; set; }
        public List<UserReport>? ReporterList { get; set; }
        public List<UserReport>? ReporteeList { get; set; }
        public List<Post>? Posts { get; set; }
        public List<Entities.Note>? Notes { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
        public List<Entities.Penalty>? Penalties { get; set; }

        // Default Values
        public static ulong DefaultId { get; set; } = ulong.MinValue;
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
        public static UserAccountStatus DefaultAccountStatus { get; set; } = UserAccountStatus.Active;
        public static ulong? DefaultCurrentEvent { get; set; } = null;
        public static bool DefaultPendingDeletion { get; set; } = false;

        // Vector
        public static int DefaultExtroversion { get; set; } = 50;
        public static int DefaultAthleticisme { get; set; } = 50;
        public static int DefaultOpenness { get; set; } = 50;
        public static int DefaultChaos { get; set; } = 50;
        public static int DefaultCompetitiveness { get; set; } = 50;
        public static int DefaultIndustriousness { get; set; } = 50;
        public static int DefaultNightOwl { get; set; } = 50;

        private static readonly CoordinateFactory Factory = new();

        //Geolocation: X = Longitude Y = Latitude     
        public static Point DefaultHaunt { get; set; } = Factory.Create(7.540, 53.483);
        public static double DefaultHauntRadius { get; set; } = 10;
        public static int DefaultHauntWheight { get; set; } = 0;
        public static Point DefaultCurrentLocation { get; set; } = Factory.Create(7.544, 53.483);
        public static double DefaultCurrentRadius { get; set; } = 10;       
    }
}
