namespace Repository
{
    internal class PenaltyFactory : Factory
    {
        internal Penalty Create(User user)
        {
            return new Penalty
            {
                PenalizedId = user.Id,
                Type = PenaltyType.Unreliable,
                Time = DateTimeOffset.MinValue,
            };
        }
    }
}
