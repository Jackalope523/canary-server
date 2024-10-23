using Core.Boundaries;

namespace Repository
{
    public class UserReport
    {
        public UserReportType Type { get; set; }

        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long? SelfId { get; init; }
        public long OtherId { get; init; }
        public long? GatheringId { get; init; }
        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }

        // Navigation Properties
        public User? Self { get; init; }
        public User? Other { get; init; }
        public Gathering? Gathering { get; init; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
