namespace Repository
{
    public class GuestClearance
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long GatheringId { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Degree { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Gathering? Gathering { get; set; }
    }
}
