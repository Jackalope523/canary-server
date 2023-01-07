namespace DataAccess.Entities
{
    internal class UserLink : Link
    {
        internal enum UserLinkType { Following, Blocked }

        public int OtherId { get; init; }
        internal User Other { get; init; }
        internal UserLinkType Type { get; set; }
       
    }
}
