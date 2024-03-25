using Core.Boundaries;

namespace Repository
{
    public class EventFactory
    {
        private int produced = 0;
        private readonly CoordinateFactory innerFactory = new();
        public Event Create(User host)
        {          
            produced++;
            return new Event
            {
                Name = "event" + produced,
                HostId = host.Id,
                Description = "This is event number " + produced + ".",
                StartTime = DateTimeOffset.UtcNow.AddHours(produced),
                GroupMinimum = 0 + produced,
                GroupMaximum = 10 + produced,
                State = EventState.Upcoming,
                Location = innerFactory.Create(7.544 + produced, 53.483 - produced),
                Radius = 10.0,
                IsDynamic = false,

                Extroversion = 8 + produced,
                Athleticisme = 2 + produced,
                Openness = 6 + produced,
                Chaos = 2 + produced,
                Competitiveness = 7 + produced,
                Industriousness = 3 + produced,
                NightOwl = 9 + produced,
            };
        }

    }

}
