using Shared;

namespace Repository
{
    public class EventLink : Link
    {
        public Event Event { get; init; }
        public EventBond Type { get; set; }
    }
}
