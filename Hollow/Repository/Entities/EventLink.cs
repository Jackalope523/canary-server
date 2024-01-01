using Shared;

namespace Repository
{
    public class EventLink : Link
    {
        internal Event Event { get; init; }
        public EventUserState Type { get; set; }
    }
}
