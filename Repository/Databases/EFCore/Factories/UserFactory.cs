using Core.Boundaries;

namespace Repository
{
    internal class UserFactory : Factory
    {
        private int produced = 0;
        private CoordinateFactory internalFactory = new();

        internal User Create()
        {
            produced++;
            return new User
            {
                PhoneNumber = "1" + new string((char)produced, 10),
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
                AccountStatus = UserAccountStatus.Active,
                NotificationId = Guid.NewGuid(),

                Extroversion = 5 + produced,
                Athleticisme = 7 + produced,
                Openness = 3 + produced,
                Chaos = 5 + produced,
                Competitiveness = 6 + produced,
                Industriousness = 1 + produced,
                NightOwl = 9 + produced,

                Haunt = internalFactory.Create(40.712, -74.006),
                HauntRadius = 10.0 + produced,
                HauntWheight = 5 + produced,
                CurrentLocation = internalFactory.Create(40.712, -74.006),
                CurrentRadius = 15.0 + produced
            };
        }
    }
}
