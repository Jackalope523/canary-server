namespace DataAccess.Entities
{
    internal class UserLink : Link
    {
        private int UserId { get; init; }
        private User User { get; init; }
        private UserLinkType Type { get; set; }

        internal UserLink(int selfId, User self, int otherId, User other, UserLinkType type) : base(selfId, self)
        {
            UserId = otherId;
            User = other;
            Type = type;
        }
    }
}
