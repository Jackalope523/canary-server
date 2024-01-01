
namespace Repository
{
    public class PostLink : Link
    {
        public enum PostLinkType { RateUp, RateDown }
        public Post Post { get; init; }
        public PostLinkType Type { get; set; }
    }
}
