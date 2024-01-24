namespace Repository
{
    public class PostLink
    {
        public enum PostLinkType { RateUp, RateDown }

        public ulong Id { get; init; }
        public ulong UserId { get; init; }
        public User User { get; init; }
        public ulong PostId { get; init; }
        public Post Post { get; init; }
        public DateTimeOffset Time { get; init; }
        public PostLinkType Type { get; set; }
    }
}
