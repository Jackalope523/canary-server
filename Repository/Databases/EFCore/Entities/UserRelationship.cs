namespace Repository
{
    public class UserRelationship
    {
        public enum UserLinkType { Appreciate, Block }

        public ulong Id { get; set; }
        public ulong SelfId { get; init; }
        public ulong OtherId { get; init; }
        public DateTimeOffset Time { get; init; }
        public UserLinkType Type { get; set; }

        // Navigation Properties
        public User? Self { get; init; }
        public User? Other { get; init; }
    }
}
