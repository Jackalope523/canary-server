using Core.Boundaries;

namespace Repository
{
    public class GatheringReport
    {
        public GatheringReportType Type { get; set; }

        public ulong Id { get; init; }

        public ulong UserId { get; init; }
        public User? Self { get; init; } // Navigation Property

        public ulong GatheringId { get; init; }
        public Gathering? Gathering { get; init; } // Navigation Property

        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }
    }
}
