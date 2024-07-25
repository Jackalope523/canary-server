namespace Repository
{
    public class Snapshot
    {
        public ulong Id { get; set; }

        public ulong OwnerId { get; set; }
        public User Owner { get; set; }
        public ulong GatheringId { get; set; }
        public Gathering Gathering { get; set; }
        public DateTimeOffset PostedAt { get; init; }
    }
}
