using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository.Entities
{
    public class Subscription
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string DeviceToken { get; set; }

        // Navigation Properties
        public User? User { get; set; }
    }
}
