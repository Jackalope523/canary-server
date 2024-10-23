namespace Repository
{
    public class Snapshot
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long OwnerId { get; set; }
        public long GatheringId { get; set; }
        public DateTimeOffset PostedAt { get; init; }

        // Navigation Properties
        public User? Owner { get; set; }
        public Gathering? Gathering { get; set; }
        public List<SnapshotReport>? Reports { get; set; }
        public List<SnapshotLink>? SnapshotLinks { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;

    }
}
