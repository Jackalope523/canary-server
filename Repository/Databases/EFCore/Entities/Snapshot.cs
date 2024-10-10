namespace Repository
{
    public class Snapshot
    {
        public ulong Id { get; set; }
        public ulong OwnerId { get; set; }
        public ulong GatheringId { get; set; }
        public DateTimeOffset PostedAt { get; init; }

        // Navigation Properties
        public User? Owner { get; set; }
        public Gathering? Gathering { get; set; }
        public List<SnapshotReport>? Reports { get; set; }
        public List<SnapshotLink>? SnapshotLinks { get; set; }

    }
}
