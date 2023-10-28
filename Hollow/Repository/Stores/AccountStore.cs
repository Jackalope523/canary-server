using Core.Boundaries;
using NetTopologySuite.Geometries;

namespace Repository
{
    public class AccountStore : QueryStore, IAccountDatabase
    {
        public static IAccountDatabase AccountDatabaseAccess => new AccountStore(new TestSentry());

        public AccountStore(Sentry sentry) : base(sentry)
        {
        }

        public bool CreateUser(string phoneNumber, string email, string normalisedEmail, string name, DateTimeOffset dateOfBirth, Character character)
        {
            User toCreate = new User
            {
                PhoneNumber = phoneNumber,
                Email = email,
                NormalizedEmail = normalisedEmail,
                Name = name,
                DateOfBirth = dateOfBirth,
                JoinDate = DateTimeOffset.UtcNow,
                Reputation = 100,
                SecurityStamp = Guid.NewGuid().ToString(),
                AccountStatus = UserAccountStatus.active,
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,
                CurrentLocation = new Point(0, 0) { SRID = 4237 },
                Haunt = new Point(0, 0) { SRID = 4237 }
            };

            storeSentry.ExecuteWrite(ctx => ctx.Users.Add(toCreate));
            return true;
        }

        public bool DeleteUser(Guid id)
        {
            storeSentry.ExecuteWrite(ctx => ctx.Users.Remove(new User { Id = id }));
            return true;
        }

        public UserShard FindUserById(Guid id) { return FindUserBy(u => u.Id == id); }
        public UserShard FindUserByPhoneNumber(string phoneNumber) { return FindUserBy(u => u.PhoneNumber == phoneNumber); }
        public UserShard FindUserByEmail(string email) { return FindUserBy(u => u.NormalizedEmail == email); }

        public (double Latitude, double Longitude, double Radius, int Stability) GetUserHaunt(Guid id)
        {
            var result = storeSentry.ExecuteRead(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new { u.Haunt.Y, u.Haunt.X, u.HauntRadius, u.HauntWheight }).Single());
            return (result.Y, result.X, result.HauntRadius, result.HauntWheight);
        }
        public (double Latitude, double Longitude, double Radius) GetRecentUserLocation(Guid id)
        {
            var result = storeSentry.ExecuteRead(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new { u.CurrentLocation.Y, u.CurrentLocation.X, u.CurrentRadius }).Single());
            return (result.Y, result.X, result.CurrentRadius);
        }    

        public bool UpdateUser(Guid id, List<(string Property, object Value)> edits)
        {
            User u = new User { Id = id };
            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u));

            foreach ((string Property, object Value) edit in edits)
            {
                switch (edit.Property)
                {
                    case "PhoneNumber":
                        u.PhoneNumber = (string)edit.Value;
                        break;
                    case "Email":
                        u.Email = (string)edit.Value;
                        break;
                    case "NormalisedEmail":
                        u.NormalizedEmail = (string)edit.Value;
                        break;
                    case "Name":
                        u.Name = (string)edit.Value;
                        break;
                    case "IsPhoneConfirmed":
                        u.IsPhoneConfirmed = (bool)edit.Value;
                        break;
                    case "IsEmailConfirmed":
                        u.IsEmailConfirmed = (bool)edit.Value;
                        break;
                    case "SecurityStamp":
                        u.SecurityStamp = (string)edit.Value;
                        break;
                    case "LockoutDate":
                        u.LockoutDate = (DateTimeOffset?)edit.Value;
                        break;
                    case "AccessTries":
                        u.AccessTries = (int)edit.Value;
                        break;
                    case "AccountStatus":
                        u.AccountStatus = (UserAccountStatus)edit.Value;
                        break;
                    case "Reputation":
                        u.Reputation = (int)edit.Value;
                        break;
                    default:
                        throw new Exception("No propertyName match found");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(edit.Property).IsModified = true);
            }
            storeSentry.ExecuteWrite();
            return true;
        }

        public bool UpdateHaunt(Guid id, double latitude, double longitude, double radius, int stability)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRecentLocation(Guid id, double latitude, double longitude, double radius)
        {
            throw new NotImplementedException();
        }


    }
}
