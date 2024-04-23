using NetTopologySuite.Geometries;
using Core.Boundaries;
using Repository.Entities;

namespace Repository
{
    internal class User
    {
        internal ulong Id { get; init; } = DefaultId;
        internal string PhoneNumber { get; set; } = DefaultPhoneNumber;
        internal string Email { get; set; } = DefaultEmail;
        internal string NormalisedEmail { get; set; } = DefaultNormalisedEmail;
        internal string Name { get; set; } = DefaultName;
        internal DateTimeOffset DateOfBirth { get; init; } = DefaultDateOfBirth;
        internal DateTimeOffset JoinDate { get; init; } = DefaultJoinDate;
        internal int Reputation { get; set; } = DefaultReputation;
        internal bool IsPhoneConfirmed { get; set; } = DefaultIsPhoneConfirmed;
        internal bool IsEmailConfirmed { get; set; } = DefaultIsEmailConfirmed;
        internal string SecurityStamp { get; set; } = DefaultSecurityStamp;
        internal DateTimeOffset? LockoutDate { get; set; } = DefaultLockoutDate;
        internal int AccessTries { get; set; } = DefaultAccessTries;
        internal UserAccountStatus AccountStatus { get; set; } = DefaultAccountStatus;
        internal ulong? CurrentEvent { get; set; } = DefaultCurrentEvent;
        internal bool IsPendingDeletion { get; set; } = DefaultIsPendingDeletion;

        // Vector
        internal int Extroversion { get; init; } = DefaultExtroversion;
        internal int Athleticisme { get; init; } = DefaultAthleticisme;
        internal int Openness { get; init; } = DefaultOpenness;
        internal int Chaos { get; init; } = DefaultChaos;
        internal int Competitiveness { get; init; } = DefaultCompetitiveness;
        internal int Industriousness { get; init; } = DefaultIndustriousness;
        internal int NightOwl { get; init; } = DefaultNightOwl;

        //Geolocation: X = Longitude Y = Latitude
        internal Point Haunt { get; set; } = DefaultHaunt;
        internal double HauntRadius { get; set; } = DefaultHauntRadius;
        internal int HauntWheight { get; set; } = DefaultHauntWheight;
        internal Point CurrentLocation { get; set; } = DefaultCurrentLocation;
        internal double CurrentRadius { get; set; } = DefaultCurrentRadius;

        // Navigation Properties
        internal List<UserLink>? UserLinks { get; set; }
        internal List<EventLink>? EventLinks { get; set; }
        internal List<PostLink>? PostLinks { get; set; }
        internal List<UserReport>? ReporterList { get; set; }
        internal List<UserReport>? ReporteeList { get; set; }
        internal List<Post>? Posts { get; set; }
        internal List<Entities.Note>? Notes { get; set; }
        internal List<Subscription>? Subscriptions { get; set; }
        internal List<Entities.Penalty>? Penalties { get; set; }

        // Default Values
        internal static ulong DefaultId { get; set; } = ulong.MinValue;
        internal static string DefaultPhoneNumber { get; set; } = "8199198013";
        internal static string DefaultEmail { get; set; } = "JohnDoe@Test.com";
        internal static string DefaultNormalisedEmail { get; set; } = "johndoe@test.com";
        internal static string DefaultName { get; set; } = "John Doe";
        internal static DateTimeOffset DefaultDateOfBirth { get; set; } = DateTimeOffset.MinValue;
        internal static DateTimeOffset DefaultJoinDate { get; set; } = DateTimeOffset.MinValue;
        internal static int DefaultReputation { get; set; } = 50;
        internal static bool DefaultIsPhoneConfirmed { get; set; } = false;
        internal static bool DefaultIsEmailConfirmed { get; set; } = false;
        internal static string DefaultSecurityStamp { get; set; } = "ijhbzdfsoiuh9ui239";
        internal static DateTimeOffset? DefaultLockoutDate { get; set; } = null;
        internal static int DefaultAccessTries { get; set; } = 3;
        internal static UserAccountStatus DefaultAccountStatus { get; set; } = UserAccountStatus.Active;
        internal static ulong? DefaultCurrentEvent { get; set; } = null;
        internal static bool DefaultIsPendingDeletion { get; set; } = false;

        // Vector
        internal static int DefaultExtroversion { get; set; } = 50;
        internal static int DefaultAthleticisme { get; set; } = 50;
        internal static int DefaultOpenness { get; set; } = 50;
        internal static int DefaultChaos { get; set; } = 50;
        internal static int DefaultCompetitiveness { get; set; } = 50;
        internal static int DefaultIndustriousness { get; set; } = 50;
        internal static int DefaultNightOwl { get; set; } = 50;

        private static readonly CoordinateFactory Factory = new();

        //Geolocation: X = Longitude Y = Latitude     
        internal static Point DefaultHaunt { get; set; } = Factory.Create(7.540, 53.483);
        internal static double DefaultHauntRadius { get; set; } = 10;
        internal static int DefaultHauntWheight { get; set; } = 0;
        internal static Point DefaultCurrentLocation { get; set; } = Factory.Create(7.544, 53.483);
        internal static double DefaultCurrentRadius { get; set; } = 10;       
    }
}
