namespace Repository
{
    internal class UserLink
    {
        internal enum UserLinkType { Follow, Block, RateUp, RateDown }

        internal ulong Id { get; set; }
        internal ulong SelfId { get; init; }
        internal User Self { get; init; }
        internal ulong OtherId { get; init; }
        internal User Other { get; init; }
        internal DateTimeOffset Time { get; init; }
        internal UserLinkType Type { get; set; }
    }
}
