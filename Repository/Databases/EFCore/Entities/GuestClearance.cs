namespace Repository
{
    public class GuestClearance
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GatheringId { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Degree { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Gathering? Gathering { get; set; }
    }
}
