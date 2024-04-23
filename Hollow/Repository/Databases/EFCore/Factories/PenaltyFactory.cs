
using Core.Boundaries;

namespace Repository
{
    internal class PenaltyFactory
    {
        internal Entities.Penalty Create(User user)
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
