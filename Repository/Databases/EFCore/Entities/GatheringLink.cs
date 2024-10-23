
namespace Repository
{
    public class GatheringLink
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long UserId { get; set; }
        public long GatheringId { get; set; }
        public DateTimeOffset Time { get; set; }
        public GatheringBond Type { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Gathering? Gathering { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
