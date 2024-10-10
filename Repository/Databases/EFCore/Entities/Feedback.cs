namespace Repository
{
    public class Feedback
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Comments { get; set; }


        // Navigation Properties
        public User? User { get; set; }
    }
}
