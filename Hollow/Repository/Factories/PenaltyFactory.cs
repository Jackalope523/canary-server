
using Core.Boundaries;

namespace Repository
{
    public class PenaltyFactory
    {
        public Entities.Penalty Create(User user)
        {
            return new Entities.Penalty
            {
                PenalizedId = user.Id,
                Type = PenaltyType.Unreliable,
                Time = DateTimeOffset.MinValue,
            };
        }
    }
}
