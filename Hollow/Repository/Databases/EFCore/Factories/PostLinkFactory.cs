
namespace Repository
{
    internal class PostLinkFactory
    {
        internal PostLink Create(User user, Post etching)
        {
            return new PostLink
            {
                UserId = user.Id,
                PostId = etching.Id,
                Type = PostLink.PostLinkType.RateUp,
                Time = DateTimeOffset.MinValue
            };
        }
    }
}
