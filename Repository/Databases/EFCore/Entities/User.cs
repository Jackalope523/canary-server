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
        public string Pseudonym { get; set; } = DefaultPseudonym;
        public DateTimeOffset DateOfBirth { get; init; } = DefaultDateOfBirth;
        public DateTimeOffset JoinDate { get; init; } = DefaultJoinDate;
        public int Reputation { get; set; } = DefaultReputation;
        public bool IsPhoneConfirmed { get; set; } = DefaultIsPhoneConfirmed;
        public bool IsEmailConfirmed { get; set; } = DefaultIsEmailConfirmed;
        public string SecurityStamp { get; set; } = DefaultSecurityStamp;
        public DateTimeOffset? LockoutDate { get; set; } = DefaultLockoutDate;
        public int AccessTries { get; set; } = DefaultAccessTries;
        public UserAccountStatus AccountStatus { get; set; } = DefaultAccountStatus;
        public ulong? CurrentGathering { get; set; } = DefaultCurrentGathering;
        public bool IsPendingDeletion { get; set; } = DefaultIsPendingDeletion;
        public DateTimeOffset TimeOfUserAgreement { get; set; } = DefaultTimeOfUserAgreement;
        public Guid NotificationId { get; set; }

        // Vector
        public int Extroversion { get; init; } = DefaultExtroversion;
        public int Athleticisme { get; init; } = DefaultAthleticisme;
        public int Openness { get; init; } = DefaultOpenness;
        public int Chaos { get; init; } = DefaultChaos;
        public int Competitiveness { get; init; } = DefaultCompetitiveness;
        public int Industriousness { get; init; } = DefaultIndustriousness;
        public int NightOwl { get; init; } = DefaultNightOwl;
        public int Age { get; init; } = DefaultAge;

        //Geolocation: X = Longitude Y = Latitude
        public Point Haunt { get; set; } = DefaultHaunt;
        public double HauntRadius { get; set; } = DefaultHauntRadius;
        public int HauntWheight { get; set; } = DefaultHauntWheight;
        public Point CurrentLocation { get; set; } = DefaultCurrentLocation;
        public double CurrentRadius { get; set; } = DefaultCurrentRadius;

        // Navigation Properties
        public List<UserRelationship>? UserLinks { get; set; }
        public List<GatheringLink>? GatheringLinks { get; set; }
        public List<SnapshotLink>? PostLinks { get; set; }
        public List<UserReport>? ReporterList { get; set; }
        public List<UserReport>? ReporteeList { get; set; }
        public List<Snapshot>? Snapshots { get; set; }
        public List<Telegram>? Notes { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
        public List<Penalty>? Penalties { get; set; }

        // Default Values
        public static ulong DefaultId { get; set; } = ulong.MinValue;
        public static string DefaultPhoneNumber { get; set; } = "8199198013";
        public static string DefaultEmail { get; set; } = "JohnDoe@Test.com";
        public static string DefaultNormalisedEmail { get; set; } = "johndoe@test.com";
        public static string DefaultName { get; set; } = "John Doe";
        public static string DefaultPseudonym { get; set; } = "JohnBoy";
        public static DateTimeOffset DefaultDateOfBirth { get; set; } = DateTimeOffset.MinValue;
        public static DateTimeOffset DefaultJoinDate { get; set; } = DateTimeOffset.MinValue;
        public static int DefaultReputation { get; set; } = 50;
        public static bool DefaultIsPhoneConfirmed { get; set; } = false;
        public static bool DefaultIsEmailConfirmed { get; set; } = false;
        public static string DefaultSecurityStamp { get; set; } = "ijhbzdfsoiuh9ui239";
        public static DateTimeOffset? DefaultLockoutDate { get; set; } = null;
        public static int DefaultAccessTries { get; set; } = 3;
        public static UserAccountStatus DefaultAccountStatus { get; set; } = UserAccountStatus.Active;
        public static ulong? DefaultCurrentGathering { get; set; } = null;
        public static bool DefaultIsPendingDeletion { get; set; } = false;
        public static DateTimeOffset DefaultTimeOfUserAgreement { get; set; } = DateTimeOffset.MinValue;

        // Vector
        public static int DefaultExtroversion { get; set; } = 50;
        public static int DefaultAthleticisme { get; set; } = 50;
        public static int DefaultOpenness { get; set; } = 50;
        public static int DefaultChaos { get; set; } = 50;
        public static int DefaultCompetitiveness { get; set; } = 50;
        public static int DefaultIndustriousness { get; set; } = 50;
        public static int DefaultNightOwl { get; set; } = 50;
        public static int DefaultAge { get; set; } = 25;

        private static readonly CoordinateFactory Factory = new();

        //Geolocation: X = Longitude Y = Latitude     
        public static Point DefaultHaunt { get; set; } = Factory.Create(7.540, 53.483);
        public static double DefaultHauntRadius { get; set; } = 10;
        public static int DefaultHauntWheight { get; set; } = 0;
        public static Point DefaultCurrentLocation { get; set; } = Factory.Create(7.544, 53.483);
        public static double DefaultCurrentRadius { get; set; } = 10;       
    }
}
