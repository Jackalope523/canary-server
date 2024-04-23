namespace Repository
{
    internal class Post
    {
        internal ulong Id { get; set; }

        internal ulong OwnerId { get; set; }
        internal User Owner { get; set; }
        internal ulong EventId { get; set; }
        internal Event Event { get; set; }
        internal DateTimeOffset PostedAt { get; init; }
        internal string PhotoURL { get; set; }
        internal bool IsHidden { get; set; }
    }
}
