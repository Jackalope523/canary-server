namespace Repository
{
    public class UserRelationship
    {
        public enum UserLinkType { Appreciate, Block }

        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long SelfId { get; init; }
        public long OtherId { get; init; }
        public DateTimeOffset Time { get; init; }
        public UserLinkType Type { get; set; }

        // Navigation Properties
        public User? Self { get; init; }
        public User? Other { get; init; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
