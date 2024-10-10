using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository.Entities
{
    public class Subscription
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string DeviceToken { get; set; }

        // Navigation Properties
        public User? User { get; set; }
    }
}
