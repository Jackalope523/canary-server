namespace Repository
{
    public abstract class Link
    {
        public Guid Id { get; init; }
        public Guid SelfId { get; init; }
        internal User Self { get; init; }
        public Guid OtherId { get; init; }

    }
}