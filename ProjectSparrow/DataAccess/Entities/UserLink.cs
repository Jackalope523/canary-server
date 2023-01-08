namespace DataAccess.Entities
{
    public class UserLink : Link
    {
        internal enum UserLinkType { Following, Blocked }

        public Guid OtherId { get; init; }
        internal User Other { get; init; }
        internal UserLinkType Type { get; set; }
       
    }
}
