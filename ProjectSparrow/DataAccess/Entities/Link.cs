namespace DataAccess.Entities
{
    internal abstract class Link
    {
        internal enum UserLinkType { Following, Blocked }
        internal enum EventLinkType { Attending, Hosting, Watching }

        internal int Id { get; init; }
        internal int UserId { get; init; }
        internal User User { get; init; }

        protected Link(int userId, User user)
        {
            UserId = userId;
            User = user;
        }
    }
}