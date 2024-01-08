using Shared;

namespace Repository.Tests
{
    internal class EventLinkFactory
    {
        int created = 0;
        public EventLink Create(User user, Event @event, EventBond type)
        {
            created++;
            return new EventLink
            {
                SelfId = user.Id,
                OtherId = @event.Id,
                Type = type,
                Time = DateTimeOffset.MinValue.AddHours(created)
            };
        }
        public EventLink Create(User user, Event @event, EventBond type, DateTimeOffset time)
        {
            created++;
            return new EventLink
            {
                SelfId = user.Id,
                OtherId = @event.Id,
                Type = type,
                Time = time
            };
        }

    }
}
