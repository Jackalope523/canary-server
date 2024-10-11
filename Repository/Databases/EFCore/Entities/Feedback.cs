namespace Repository
{
    public class Feedback
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Comments { get; set; }


        // Navigation Properties
        public User? User { get; set; }
    }
}
