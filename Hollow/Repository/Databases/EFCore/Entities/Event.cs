using NetTopologySuite.Geometries;
using Core.Boundaries;

namespace Repository
{
    internal class Event
    {
        internal ulong Id { get; set; }

        internal string Name { get; set; } = DefaultName;
        internal string Description { get; set; } = DefaultDescription;
        internal DateTimeOffset StartTime { get; set; } = DefaultStartTime;
        internal ulong HostId { get; set; } = DefaultHostId;
        internal User? Host { get; set; }

        // X = Longitude Y = Latitude
        internal Point Location { get; set; } = DefaultLocation;

        internal EventState State { get; set; } = DefaultState;
        internal int GroupMinimum { get; set; } = DefaultGroupMinimum;
        internal int GroupMaximum { get; set; } = DefaultGroupMaximum;
        internal DateTimeOffset? EndTime { get; set; }
        internal double Radius { get; set; } = DefaultRadius;
        internal bool IsDynamic { get; set; } = DefaultIsDynamic;
        internal bool IsPendingDeletion { get; set; } = DefaultIsPendingDeletion;
        internal int NumberOfGuests { get; set; } = DefaultNumberOfGuests;

        // Vector
        internal int Extroversion { get; init; } = DefaultExtroversion;
        internal int Athleticisme { get; init; } = DefaultAthleticisme;
        internal int Openness { get; init; } = DefaultOpenness;
        internal int Chaos { get; init; } = DefaultChaos;
        internal int Competitiveness { get; init; } = DefaultCompetitiveness;
        internal int Industriousness { get; init; } = DefaultIndustriousness;
        internal int NightOwl { get; init; } = DefaultNightOwl;

        // Navigation Properties
        internal List<EventLink>? Links { get; set; }
        internal List<EventReport>? Reports { get; set; }
        internal List<Post>? Posts { get; set; }

        // Default Values
        private static readonly CoordinateFactory Factory = new();

        internal static ulong DefaultId { get; set; } = ulong.MinValue;

        internal static string DefaultName { get; set; } = "";
        internal static string DefaultDescription { get; set; } = "";
        internal static DateTimeOffset DefaultStartTime { get; set; } = DateTimeOffset.MinValue;
        internal static ulong DefaultHostId { get; set; } = ulong.MinValue;
        internal static Point DefaultLocation { get; set; } = Factory.Create(7.544, 53.483);

        internal static EventState DefaultState { get; set; } = EventState.Upcoming;
        internal static int DefaultGroupMinimum { get; set; } = 0;
        internal static int DefaultGroupMaximum { get; set; } = 10;
        internal static double DefaultRadius { get; set; } = 10.000;
        internal static bool DefaultIsDynamic { get; set; } = false;
        internal static bool DefaultIsPendingDeletion { get; set; } = false;
        internal static int DefaultNumberOfGuests { get; set; } = 0;

        // Vector
        internal static int DefaultExtroversion { get; set; } = 50;
        internal static int DefaultAthleticisme { get; set; } = 50;
        internal static int DefaultOpenness { get; set; } = 50;
        internal static int DefaultChaos { get; set; } = 50;
        internal static int DefaultCompetitiveness { get; set; } = 50;
        internal static int DefaultIndustriousness { get; set; } = 50;
        internal static int DefaultNightOwl { get; set; } = 50;
    }
}
