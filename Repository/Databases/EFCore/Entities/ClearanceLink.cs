namespace Repository
{
    public class ClearanceLink
    {
        public enum ClearanceType { Guest }

        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public User? User { get; set; }
        public ulong GatheringId { get; set; }
        public Gathering? Gathering { get; set; }
        public DateTimeOffset Time { get; set; }
        public ClearanceType Type { get; set; }
    }
}
