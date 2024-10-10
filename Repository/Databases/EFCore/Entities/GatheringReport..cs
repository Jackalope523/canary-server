using Core.Boundaries;

namespace Repository
{
    public class GatheringReport
    {
        public GatheringReportType Type { get; set; }

        public ulong Id { get; init; }
        public ulong UserId { get; init; }
        public ulong GatheringId { get; init; }
        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }

        // Navigation Properties
        public User? User { get; init; }
        public Gathering? Gathering { get; init; }
    }
}
