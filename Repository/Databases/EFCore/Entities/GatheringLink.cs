
namespace Repository
{
    public class GatheringLink
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long GatheringId { get; set; }
        public DateTimeOffset Time { get; set; }
        public GatheringBond Type { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Gathering? Gathering { get; set; }

    }
}
