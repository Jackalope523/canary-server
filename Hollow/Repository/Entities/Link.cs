namespace Repository
{
    public abstract class Link
    {
        public ulong Id { get; init; }
        public ulong SelfId { get; init; }
        public User Self { get; init; }
        public ulong OtherId { get; init; }
        public DateTimeOffset Time { get; init; }

    }
}