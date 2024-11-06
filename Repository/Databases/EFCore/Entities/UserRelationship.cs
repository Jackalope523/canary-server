namespace Repository
{
    public class UserRelationship : Entity
    {
        public enum UserLinkType { Appreciate, Block }

        public long SelfId { get; init; }
        public long OtherId { get; init; }
        public DateTimeOffset Time { get; init; }
        public UserLinkType Type { get; set; }

        // Navigation Properties
        public User? Self { get; init; }
        public User? Other { get; init; }

        // Default Values
    }
}
