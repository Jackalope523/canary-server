
namespace Repository.Tests
{
    internal class PostLinkFactory
    {
        public PostLink Create(User user, Post etching)
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
