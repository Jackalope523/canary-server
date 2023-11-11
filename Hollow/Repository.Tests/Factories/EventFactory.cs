using NetTopologySuite.Geometries;

namespace Repository
{
    internal class EventFactory
    {
        private int produced = 0;

        public Event Create()
        {
            produced++;
            return new Event
            {
                Name = "event" + produced,
                Description = "This is event number " + produced + ".",
                StartTime = DateTimeOffset.UtcNow.AddHours(produced),
                GroupMinimum = 0 + produced,
                GroupMaximum = 10 + produced,
                IsOpen = false,
                Location = new Point(34.0522, -118.2437),

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
