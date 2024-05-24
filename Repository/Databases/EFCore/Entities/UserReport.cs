using Core.Boundaries;

namespace Repository
{
    public class UserReport
    {
        public UserReportType Type { get; set; }

        public ulong Id { get; init; }

        public ulong SelfId { get; init; }
        public User? Self { get; init; } // Navigation Property

        public ulong OtherId { get; init; }
        public User? Other { get; init; } // Navigation Property

        public ulong? GatheringId { get; init; }
        public Gathering? Gathering { get; init; } // Navigation Property

        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }
    }
}
