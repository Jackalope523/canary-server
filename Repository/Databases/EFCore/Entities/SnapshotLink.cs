namespace Repository
{
    public class SnapshotLink
    {
        public enum SnapshotLinkType { RateUp, RateDown }

        public ulong Id { get; set; }
        public ulong UserId { get; init; }
        public User User { get; init; }
        public ulong PostId { get; init; }
        public Snapshot Post { get; init; }
        public DateTimeOffset Time { get; init; }
        public SnapshotLinkType Type { get; set; }
    }
}
