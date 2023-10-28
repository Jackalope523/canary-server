
namespace Repository
{
    public class PostLink : Link
    {
        public enum PostLinkType { RateUp, RateDown }

        public Guid PostId { get; init; }
        internal Post Post { get; init; }
        public PostLinkType Type { get; set; }
    }
}
