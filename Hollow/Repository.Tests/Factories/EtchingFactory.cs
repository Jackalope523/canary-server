
namespace Repository.Tests
{
    internal class EtchingFactory
    {
        private int created = 0;
        
        public Post Create(User owner, Event location)
        {
            created++;
            return new Post
            {
                OwnerId = owner.Id,
                EventId = location.Id,
                PostedAt = DateTime.MinValue,
                PhotoURL = "URL " + created,
                IsHidden = false
            };
        }
    }
}
