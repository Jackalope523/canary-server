using NetTopologySuite.Geometries;

namespace Repository
{
    public class Gathering
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public string Title { get; set; } = DefaultTitle;
        public string Description { get; set; } = DefaultDescription;
        public DateTimeOffset StartTime { get; set; } = DefaultStartTime;
        public long? HostId { get; set; } = DefaultHostId;

        // X = Longitude Y = Latitude
        public Point Location { get; set; } = DefaultLocation;
        public string FriendlyLocation { get; set; } = DefaultFriendlyLocation;

        public GatheringState State { get; set; } = DefaultState;
        public int GroupMinimum { get; set; } = DefaultGroupMinimum;
        public int GroupMaximum { get; set; } = DefaultGroupMaximum;
        public DateTimeOffset? EndTime { get; set; }
        public double Radius { get; set; } = DefaultRadius;
        public bool IsDynamic { get; set; } = DefaultIsDynamic;
        public int NumberOfGuests { get; set; } = DefaultNumberOfGuests;
        public int DegreeOfPrivacy { get; set; } = DefaultDegreeOfPrivacy;

        // Vector
        public int Extroversion { get; init; } = DefaultExtroversion;
        public int Athleticisme { get; init; } = DefaultAthleticisme;
        public int Openness { get; init; } = DefaultOpenness;
        public int Chaos { get; init; } = DefaultChaos;
        public int Competitiveness { get; init; } = DefaultCompetitiveness;
        public int Industriousness { get; init; } = DefaultIndustriousness;
        public int NightOwl { get; init; } = DefaultNightOwl;
        public int Age { get; init; } = DefaultAge;

        // Navigation Properties
        public User? Host { get; set; }
        public List<GatheringLink>? GatheringLink { get; set; }
        public List<GatheringReport>? GatheringReports { get; set; }
        public List<UserReport>? UserReports { get; set; }
        public List<Snapshot>? Snapshots { get; set; }
        public List<GuestClearance>? GuestClearances { get; set; }

        // Default Values
        private static readonly CoordinateFactory Factory = new();

        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
        public static string DefaultHeroImageURL { get; set; } = "";
        public static string DefaultTitle { get; set; } = "Lewis";
        public static string DefaultDescription { get; set; } = "A dog named Lewis.";
        public static DateTimeOffset DefaultStartTime { get; set; } = DateTimeOffset.MinValue;
        public static long? DefaultHostId { get; set; } = null;
        public static Point DefaultLocation { get; set; } = Factory.Create(7.544, 53.483);
        public static string DefaultFriendlyLocation { get; set; } = "Solitude";
        public static GatheringState DefaultState { get; set; } = GatheringState.Upcoming;
        public static int DefaultGroupMinimum { get; set; } = 0;
        public static int DefaultGroupMaximum { get; set; } = 10;
        public static double DefaultRadius { get; set; } = 10.000;
        public static bool DefaultIsDynamic { get; set; } = false;
        public static int DefaultNumberOfGuests { get; set; } = 0;
        public static int DefaultDegreeOfPrivacy { get; set; } = 3;

        // Vector
        public static int DefaultExtroversion { get; set; } = 50;
        public static int DefaultAthleticisme { get; set; } = 50;
        public static int DefaultOpenness { get; set; } = 50;
        public static int DefaultChaos { get; set; } = 50;
        public static int DefaultCompetitiveness { get; set; } = 50;
        public static int DefaultIndustriousness { get; set; } = 50;
        public static int DefaultNightOwl { get; set; } = 50;
        public static int DefaultAge { get; set; } = 25;
    }
}
