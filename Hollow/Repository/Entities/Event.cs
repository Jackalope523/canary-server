using NetTopologySuite.Geometries;
using Core.Boundaries;

namespace Repository
{
    public class Event
    {
        public ulong Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public ulong HostId { get; set; }
        internal User Host { get; set; } 
        public Point Location { get; set; } // X = Longitude Y = Latitude

        public EventState State { get; set; } = EventState.Open;
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public double Radius { get; set; }
        public bool IsDynamic { get; set; }

        // Vector
        public int Extroversion { get; init; }
        public int Athleticisme { get; init; }
        public int Openness { get; init; }
        public int Chaos { get; init; }
        public int Competitiveness { get; init; }
        public int Industriousness { get; init; }
        public int NightOwl { get; init; }

        // Navigation Properties
        public List<EventLink> Links { get; set; }
        public List<EventReport> Reports { get; set; }
        public List<Post> Posts { get; set; }

        // Default Values
        private static readonly CoordinateFactory Factory = new();

        public static ulong DefaultId { get; set; } = ulong.MinValue;

        public static string DefaultName { get; set; } = "Default Event";
        public static string DefaultDescription { get; set; } = "Default Description";
        public static DateTimeOffset DefaultStartTime { get; set; } = DateTimeOffset.MinValue;
        public static ulong DefaultHostId { get; set; } = ulong.MinValue;
        public static Point DefaultLocation { get; set; } = Factory.Create(40.712, -74.006);

        public static EventState DefaultState { get; set; } = EventState.Open;
        public static int DefaultGroupMinimum { get; set; } = 0;
        public static int DefaultGroupMaximum { get; set; } = 10;
        public static double DefaultRadius { get; set; } = 10.000;
        public static bool DefaultIsDynamic { get; set; } = false;

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
