namespace Repository
{
    public class UserLink : Link
    {
        public enum UserLinkType { Follow, Block, RateUp, RateDown }

        internal User Other { get; init; }
        public UserLinkType Type { get; set; }
    }
}
