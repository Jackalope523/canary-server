namespace Repository
{
    internal class UserLinkFactory
    {      
        internal UserRelationship Create(User self, User other, UserRelationship.UserLinkType type)
        {
            return new UserRelationship
            {
                SelfId = self.Id,
                OtherId = other.Id,
                Type = type,
                Time = DateTimeOffset.MinValue
            };
        }
    }
}
