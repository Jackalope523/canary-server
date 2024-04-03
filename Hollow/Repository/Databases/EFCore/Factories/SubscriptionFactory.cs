using Repository.Entities;

namespace Repository
{
    public class SubscriptionFactory
    {
        private int produced = 0;

        public Subscription Create(User user)
        {
            produced++;
            return new Subscription
            {
                UserId = user.Id,
                DeviceType = Shared.DeviceType.iOS,
                DeviceToken = produced.ToString()
            };
        }
    }
}
