namespace Repository
{
    public class SnapshotLink
    {
        public enum SnapshotLinkType { Appreciate }

        public long Id { get; set; }
        public long UserId { get; init; }
        public long SnapshotId { get; init; }
        public DateTimeOffset Time { get; init; }
        public SnapshotLinkType Type { get; set; }

        // Navigation Properties
        public User? User { get; init; }
        public Snapshot? Snapshot { get; init; }
    }
}
