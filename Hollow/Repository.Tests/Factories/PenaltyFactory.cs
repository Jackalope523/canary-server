
using Core.Boundaries;

namespace Repository
{
    internal class PenaltyFactory
    {
        public Entities.Penalty Create(User user)
        {
            return new Entities.Penalty
            {
                PenalizedId = user.Id,
                Type = PenaltyType.Unreliable,
                Time = DateTime.MinValue,
            };
        }
    }
}
