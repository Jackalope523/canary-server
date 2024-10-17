using Core.Boundaries;

namespace Repository
{
    internal class GatheringFactory
    {
        private int produced = 0;
        private readonly CoordinateFactory innerFactory = new();
        internal Gathering Create(User host)
        {          
            produced++;
            return new Gathering
            {
                Name = "gathering" + produced,
                HostId = host.Id,
                Description = "This is gathering number " + produced + ".",
                StartTime = DateTimeOffset.UtcNow.AddHours(produced),
                GroupMinimum = 0 + produced,
                GroupMaximum = 10 + produced,
                State = GatheringState.Upcoming,
                Location = innerFactory.Create(17.544 + produced, -72.483 - produced),
                Radius = 10.0,
                IsDynamic = false,
                DegreeOfPrivacy = 3,

                Extroversion = 8 + produced,
                Athleticisme = 2 + produced,
                Openness = 6 + produced,
                Chaos = 2 + produced,
                Competitiveness = 7 + produced,
                Industriousness = 3 + produced,
                NightOwl = 9 + produced,
            };
        }

        internal Gathering Create()
        {
            produced++;
            return new Gathering
            {
                Name = "gathering" + produced,
                HostId = 0,
                Description = "This is gathering number " + produced + ".",
                StartTime = DateTimeOffset.UtcNow.AddHours(produced),
                GroupMinimum = 0 + produced,
                GroupMaximum = 10 + produced,
                State = GatheringState.Upcoming,
                Location = innerFactory.Create(17.544 + produced, -72.483 - produced),
                Radius = 10.0,
                IsDynamic = false,
                DegreeOfPrivacy = 3,

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
