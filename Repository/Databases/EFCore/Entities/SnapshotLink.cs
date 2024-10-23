namespace Repository
{
    public class SnapshotLink
    {
        public enum SnapshotLinkType { Appreciate }

        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long UserId { get; init; }
        public long SnapshotId { get; init; }
        public DateTimeOffset Time { get; init; }
        public SnapshotLinkType Type { get; set; }

        // Navigation Properties
        public User? User { get; init; }
        public Snapshot? Snapshot { get; init; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
