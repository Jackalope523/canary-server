using NetTopologySuite.Geometries;

namespace Repository
{
    public class Gathering : Entity
    {
        public string Title { get; set; } = DefaultTitle;
        public string Description { get; set; } = DefaultDescription;
        public DateTimeOffset StartTime { get; set; } = DefaultStartTime;
        public DateTimeOffset TimeOfCreation { get; set; } = DefaultTimeOfCreation;
        public long? HostId { get; set; } = DefaultHostId;

        // X = Longitude Y = Latitude
        public Point Location { get; set; } = DefaultLocation;
        public string FriendlyLocation { get; set; } = DefaultFriendlyLocation;

        public GatheringState State { get; set; } = DefaultState;
        public GatheringVisibility Visibility { get; set; } = DefaultVisibility;
        public int GroupMinimum { get; set; } = DefaultGroupMinimum;
        public int GroupMaximum { get; set; } = DefaultGroupMaximum;
        public DateTimeOffset? EndTime { get; set; }
        public double Radius { get; set; } = DefaultRadius;
        public bool IsDynamic { get; set; } = DefaultIsDynamic;
        public int NumberOfGuests { get; set; } = DefaultNumberOfGuests;
        public int DegreeOfPrivacy { get; set; } = DefaultDegreeOfPrivacy;
        public float Decay { get; set; } = DefaultDecay;

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
        public GatheringChat? Chat { get; set; }
        public List<GatheringLink>? GatheringLink { get; set; }
        public List<GatheringReport>? GatheringReports { get; set; }
        public List<UserReport>? UserReports { get; set; }
        public List<Snapshot>? Snapshots { get; set; }
        public List<GuestClearance>? GuestClearances { get; set; }
        public List<Notification>? Notifications { get; set; }
        public List<GatheringShareMessage>? Shares { get; set; }
        public List<GatheringInviteMessage>? Invites { get; set; }

        // Default Values
        private static readonly CoordinateFactory Factory = new();

        public static string DefaultHeroImageURL { get; set; } = "";
        public static string DefaultTitle { get; set; } = "Lewis";
        public static string DefaultDescription { get; set; } = "A dog named Lewis.";
        public static DateTimeOffset DefaultStartTime { get; set; } = DateTimeOffset.MinValue;
        public static DateTimeOffset DefaultTimeOfCreation { get; set; } = DateTimeOffset.MinValue;
        public static long? DefaultHostId { get; set; } = null;
        public static Point DefaultLocation { get; set; } = Factory.Create(7.544, 53.483);
        public static string DefaultFriendlyLocation { get; set; } = "Solitude";
        public static GatheringState DefaultState { get; set; } = GatheringState.Alive;
        public static GatheringVisibility DefaultVisibility { get; set; } = GatheringVisibility.Visible;
        public static int DefaultGroupMinimum { get; set; } = 0;
        public static int DefaultGroupMaximum { get; set; } = 10;
        public static double DefaultRadius { get; set; } = 10.000;
        public static bool DefaultIsDynamic { get; set; } = false;
        public static int DefaultNumberOfGuests { get; set; } = 0;
        public static int DefaultDegreeOfPrivacy { get; set; } = 3;
        public static float DefaultDecay { get; set; } = 100;

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
