namespace Repository
{
    public class UserLink : Link
    {
        public enum UserLinkType { Follow, Block, RateUp, RateDown }

        public User Other { get; init; }
        public UserLinkType Type { get; set; }
    }
}
