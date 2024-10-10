namespace Repository
{
    public class SnapshotLink
    {
        public enum SnapshotLinkType { Appreciate }

        public ulong Id { get; set; }
        public ulong UserId { get; init; }
        public ulong SnapshotId { get; init; }
        public DateTimeOffset Time { get; init; }
        public SnapshotLinkType Type { get; set; }

        // Navigation Properties
        public User? User { get; init; }
        public Snapshot? Snapshot { get; init; }
    }
}
