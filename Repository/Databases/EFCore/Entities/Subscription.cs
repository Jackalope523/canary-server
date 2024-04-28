using Core.Boundaries;

namespace Repository.Entities
{
    internal class Subscription
    {
        internal ulong Id { get; set; }

        internal ulong UserId { get; set; }
        internal User User { get; set; }
        internal DeviceType DeviceType { get; set; }
        internal string DeviceToken { get; set; }
    }
}
