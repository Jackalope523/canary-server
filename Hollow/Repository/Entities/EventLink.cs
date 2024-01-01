using Shared;

namespace Repository
{
    public class EventLink : Link
    {
        public Event Event { get; init; }
        public EventUserState Type { get; set; }
    }
}
