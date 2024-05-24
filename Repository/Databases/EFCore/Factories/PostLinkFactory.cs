
namespace Repository
{
    internal class PostLinkFactory
    {
        internal PostLink Create(User user, Post snapshot)
        {
            return new PostLink
            {
                UserId = user.Id,
                PostId = snapshot.Id,
                Type = PostLink.PostLinkType.RateUp,
                Time = DateTimeOffset.MinValue
            };
        }
    }
}
