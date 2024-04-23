namespace Repository
{
    internal class UserLinkFactory
    {      
        internal UserLink Create(User self, User other, UserLink.UserLinkType type)
        {
            return new UserLink
            {
                SelfId = self.Id,
                OtherId = other.Id,
                Type = type,
                Time = DateTimeOffset.MinValue
            };
        }
    }
}
