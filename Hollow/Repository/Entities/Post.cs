namespace Repository
{
    public class Post
    {
        public ulong Id { get; set; }

        public ulong OwnerId { get; set; }
        internal User Owner { get; set; }
        public ulong EventId { get; set; }
        internal Event Event { get; set; }
        public DateTimeOffset PostedAt { get; init; }
        public string PhotoURL { get; set; }
        public bool IsHidden { get; set; }
    }
}
