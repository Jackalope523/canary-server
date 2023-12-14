
namespace Repository.Tests
{
    internal class PostLinkFactory
    {
        public PostLink Create(User user, Post etching)
        {
            return new PostLink
            {
                SelfId = user.Id,
                OtherId = etching.Id,
                Type = PostLink.PostLinkType.RateUp,
            };
        }
    }
}
