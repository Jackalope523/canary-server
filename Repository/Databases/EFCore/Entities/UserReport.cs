using Core.Boundaries;

namespace Repository
{
    public class UserReport
    {
        public UserReportType Type { get; set; }

        public ulong Id { get; init; }
        public ulong SelfId { get; init; }
        public ulong OtherId { get; init; }
        public ulong? GatheringId { get; init; }
        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }

        // Navigation Properties
        public User? Self { get; init; }
        public User? Other { get; init; }
        public Gathering? Gathering { get; init; }
    }
}
