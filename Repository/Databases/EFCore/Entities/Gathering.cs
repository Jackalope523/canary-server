using NetTopologySuite.Geometries;
using Core.Boundaries;

namespace Repository
{
    public class Gathering
    {
        public ulong Id { get; set; }

        public string HeroImageURL { get; set; } = DefaultHeroImageURL;
        public string Name { get; set; } = DefaultName;
        public string Description { get; set; } = DefaultDescription;
        public DateTimeOffset StartTime { get; set; } = DefaultStartTime;
        public ulong HostId { get; set; } = DefaultHostId;
        public User? Host { get; set; }

        // X = Longitude Y = Latitude
        public Point Location { get; set; } = DefaultLocation;
        public string FriendlyLocation { get; set; } = DefaultFriendlyLocation;

        public GatheringState State { get; set; } = DefaultState;
        public int GroupMinimum { get; set; } = DefaultGroupMinimum;
        public int GroupMaximum { get; set; } = DefaultGroupMaximum;
        public DateTimeOffset? EndTime { get; set; }
        public double Radius { get; set; } = DefaultRadius;
        public bool IsDynamic { get; set; } = DefaultIsDynamic;
        public bool IsPendingDeletion { get; set; } = DefaultIsPendingDeletion;
        public int NumberOfGuests { get; set; } = DefaultNumberOfGuests;

        // Vector
        public int Extroversion { get; init; } = DefaultExtroversion;
        public int Athleticisme { get; init; } = DefaultAthleticisme;
        public int Openness { get; init; } = DefaultOpenness;
        public int Chaos { get; init; } = DefaultChaos;
        public int Competitiveness { get; init; } = DefaultCompetitiveness;
        public int Industriousness { get; init; } = DefaultIndustriousness;
        public int NightOwl { get; init; } = DefaultNightOwl;

        // Navigation Properties
        public List<GatheringLink>? Links { get; set; }
        public List<GatheringReport>? Reports { get; set; }
        public List<Snapshot>? Posts { get; set; }

        // Default Values
        private static readonly CoordinateFactory Factory = new();

        public static ulong DefaultId { get; set; } = ulong.MinValue;

        public static string DefaultHeroImageURL { get; set; } = "";
        public static string DefaultName { get; set; } = "Lewis";
        public static string DefaultDescription { get; set; } = "A dog named Lewis.";
        public static DateTimeOffset DefaultStartTime { get; set; } = DateTimeOffset.MinValue;
        public static ulong DefaultHostId { get; set; } = ulong.MinValue;
        public static Point DefaultLocation { get; set; } = Factory.Create(7.544, 53.483);
        public static string DefaultFriendlyLocation { get; set; } = "Solitude";

        public static GatheringState DefaultState { get; set; } = GatheringState.Upcoming;
        public static int DefaultGroupMinimum { get; set; } = 0;
        public static int DefaultGroupMaximum { get; set; } = 10;
        public static double DefaultRadius { get; set; } = 10.000;
        public static bool DefaultIsDynamic { get; set; } = false;
        public static bool DefaultIsPendingDeletion { get; set; } = false;
        public static int DefaultNumberOfGuests { get; set; } = 0;

        // Vector
        public static int DefaultExtroversion { get; set; } = 50;
        public static int DefaultAthleticisme { get; set; } = 50;
        public static int DefaultOpenness { get; set; } = 50;
        public static int DefaultChaos { get; set; } = 50;
        public static int DefaultCompetitiveness { get; set; } = 50;
        public static int DefaultIndustriousness { get; set; } = 50;
        public static int DefaultNightOwl { get; set; } = 50;
    }
}
