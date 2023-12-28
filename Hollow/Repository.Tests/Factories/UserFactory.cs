using Core.Boundaries;
using NetTopologySuite.Geometries;

namespace Repository
{
    internal class UserFactory
    {
        private int produced = 0;

        public User Create()
        {
            produced++;
            return new User
            {
                PhoneNumber = new string((char)produced, 3) + "-" + new string((char)produced, 3) + "-" + new string((char)produced, 4),
                Email = "email_" + produced + "@test.com",
                Name = "user" + produced,
                DateOfBirth = DateTimeOffset.Now.AddDays(-(produced + 1000)),
                JoinDate = DateTimeOffset.Now.AddDays(-365),
                Reputation = 100 - produced,
                NormalisedEmail = "email_" + produced + "@test.com",
                IsPhoneConfirmed = false,
                IsEmailConfirmed = false,
                SecurityStamp = "stamp" + produced,
                LockoutDate = DateTimeOffset.Now.AddDays(-365),
                AccessTries = 3 + produced,
                AccountStatus = UserAccountStatus.active,

                Extroversion = 5 + produced,
                Athleticisme = 7 + produced,
                Openness = 3 + produced,
                Chaos = 5 + produced,
                Competitiveness = 6 + produced,
                Industriousness = 1 + produced,
                NightOwl = 9 + produced,

                Haunt = new Point(40.7128, -74.0060),
                HauntRadius = 10.0 + produced,
                HauntWheight = 5 + produced,
                CurrentLocation = new Point(40.7128, -74.0060),
                CurrentRadius = 15.0 + produced
            };
        }

    }
}
