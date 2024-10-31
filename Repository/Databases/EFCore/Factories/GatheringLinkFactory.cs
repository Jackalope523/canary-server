using Core.Boundaries;

namespace Repository
{
    internal class GatheringLinkFactory : Factory
    {
        int created = 0;
        internal GatheringLink Create(User user, Gathering gathering, GatheringBond type)
        {
            created++;
            return new GatheringLink
            {
                UserId = user.Id,
                GatheringId = gathering.Id,
                Type = type,
                Time = DateTimeOffset.MinValue.AddHours(created)
            };
        }
        internal GatheringLink Create(User user, Gathering gathering, GatheringBond type, DateTimeOffset time)
        {
            created++;
            return new GatheringLink
            {
                UserId = user.Id,
                GatheringId = gathering.Id,
                Type = type,
                Time = time
            };
        }

    }
}
