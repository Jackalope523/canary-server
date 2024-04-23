namespace Repository
{
    internal class PostLink
    {
        internal enum PostLinkType { RateUp, RateDown }

        internal ulong Id { get; set; }
        internal ulong UserId { get; init; }
        internal User User { get; init; }
        internal ulong PostId { get; init; }
        internal Post Post { get; init; }
        internal DateTimeOffset Time { get; init; }
        internal PostLinkType Type { get; set; }
    }
}
