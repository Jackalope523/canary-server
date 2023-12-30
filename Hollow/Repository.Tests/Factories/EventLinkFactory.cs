using Shared;

namespace Repository.Tests
{
    internal class EventLinkFactory
    {      
        public EventLink Create(User user, Event @event, EventUserState type)
        {
            return new EventLink
            {
                SelfId = user.Id,
                OtherId = @event.Id,
                Type = type,
                Time = DateTimeOffset.MinValue,
            };
        }
    }
}
