namespace Repository
{
    public class SnapshotReport
    {
        public SnapshotReportType Type { get; set; }
        public ulong Id { get; init; }
        public ulong UserId { get; init; }
        public ulong SnapshotId { get; init; }
        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }

        // Navigation Properties
        public User? User { get; init; }
        public Snapshot? Snapshot { get; init; }
    }
}
