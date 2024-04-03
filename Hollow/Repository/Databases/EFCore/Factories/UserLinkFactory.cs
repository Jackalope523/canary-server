namespace Repository
{
    public class UserLinkFactory
    {      
        public UserLink Create(User self, User other, UserLink.UserLinkType type)
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
