namespace Repository
{
    public class UserLink
    {
        public enum UserLinkType { Appreciate, Block }

        public ulong Id { get; set; }
        public ulong SelfId { get; init; }
        public User Self { get; init; }
        public ulong OtherId { get; init; }
        public User Other { get; init; }
        public DateTimeOffset Time { get; init; }
        public UserLinkType Type { get; set; }
    }
}
