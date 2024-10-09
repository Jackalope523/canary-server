using Repository.Entities;

namespace Repository
{
    internal class SubscriptionFactory
    {
        private int produced = 0;

        internal Subscription Create(User user)
        {
            produced++;
            return new Subscription
            {
                UserId = user.Id,
            };
        }
    }
}
