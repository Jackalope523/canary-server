namespace Repository
{
    public class Post
    {
        public ulong Id { get; set; }

        public ulong OwnerId { get; set; }
        public User Owner { get; set; }
        public ulong EventId { get; set; }
        public Event Event { get; set; }
        public DateTimeOffset PostedAt { get; init; }
        public string PhotoURL { get; set; }
        public bool IsHidden { get; set; }
    }
}
