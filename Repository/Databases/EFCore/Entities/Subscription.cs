using Core.Boundaries;

namespace Repository.Entities
{
    public class Subscription
    {
        public ulong Id { get; set; }

        public ulong UserId { get; set; }
        public User User { get; set; }
        public string DeviceToken { get; set; }
    }
}
