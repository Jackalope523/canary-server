namespace Repository
{
    public class SnapshotReport
    {
        public SnapshotReportType Type { get; set; }

        public ulong Id { get; init; }

        public ulong UserId { get; init; }
        public User? User { get; init; } // Navigation Property

        public ulong SnapshotId { get; init; }
        public Snapshot? Snapshot { get; init; } // Navigation Property

        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }
    }
}
