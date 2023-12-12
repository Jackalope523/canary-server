using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Repository
{
    public class AccountStore : QueryStore, IAccountDatabase
    {
        public static IAccountDatabase AccountDatabaseAccess => new AccountStore(new TestSentry());

        public AccountStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<bool> CreateUserAsync(string phoneNumber, string email, string normalisedEmail, string name, DateTimeOffset dateOfBirth, Character character)
        {
            User toCreate = new User
            {
                PhoneNumber = phoneNumber,
                Email = email,
                NormalisedEmail = normalisedEmail,
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

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.Add(toCreate));
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.Remove(new User { Id = id }));
            return true;
        }

        public async Task<UserShard> FindUserByIdAsync(Guid id) 
        {            
            int numFollowers;

            UserShard user = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new UserShard
               (
                   u.Id,
                   u.PhoneNumber,
                   u.Email,
                   u.Name,
                   u.DateOfBirth,
                   u.IsPhoneConfirmed,
                   u.IsEmailConfirmed,
                   u.SecurityStamp,
                   u.LockoutDate,
                   u.AccessTries,
                   u.AccountStatus,
                   u.JoinDate,
                   u.Reputation,
                   -1,
                   new Character(
                   u.Extroversion,
                   u.Athleticisme,
                   u.Chaos,
                   u.Competitiveness,
                   u.Industriousness,
                   u.NightOwl,
                   u.Openness)
               )).SingleAsync());

            numFollowers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).CountAsync());

            return user with { NumberOfFollowers = numFollowers };
        }
        public async Task<UserShard> FindUserByPhoneNumberAsync(string phoneNumber) 
        { 
            int numFollowers;

            UserShard user = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.PhoneNumber == phoneNumber).Select(u => new UserShard
               (
                   u.Id,
                   u.PhoneNumber,
                   u.Email,
                   u.Name,
                   u.DateOfBirth,
                   u.IsPhoneConfirmed,
                   u.IsEmailConfirmed,
                   u.SecurityStamp,
                   u.LockoutDate,
                   u.AccessTries,
                   u.AccountStatus,
                   u.JoinDate,
                   u.Reputation,
                   -1,
                   new Character(
                   u.Extroversion,
                   u.Athleticisme,
                   u.Chaos,
                   u.Competitiveness,
                   u.Industriousness,
                   u.NightOwl,
                   u.Openness)
               )).SingleAsync());

            numFollowers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).CountAsync());

            return user with { NumberOfFollowers = numFollowers };
        }
        public async Task<UserShard> FindUserByEmailAsync(string email) 
        { 
            int numFollowers;

            UserShard user = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Email == email).Select(u => new UserShard
               (
                   u.Id,
                   u.PhoneNumber,
                   u.Email,
                   u.Name,
                   u.DateOfBirth,
                   u.IsPhoneConfirmed,
                   u.IsEmailConfirmed,
                   u.SecurityStamp,
                   u.LockoutDate,
                   u.AccessTries,
                   u.AccountStatus,
                   u.JoinDate,
                   u.Reputation,
                   -1,
                   new Character(
                   u.Extroversion,
                   u.Athleticisme,
                   u.Chaos,
                   u.Competitiveness,
                   u.Industriousness,
                   u.NightOwl,
                   u.Openness)
               )).SingleAsync());

            numFollowers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).CountAsync());

            return user with { NumberOfFollowers = numFollowers };
        }

        public async Task<(double Latitude, double Longitude, double Radius, int Stability)> GetUserHauntAsync(Guid id)
        {
            var result = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new { u.Haunt.Y, u.Haunt.X, u.HauntRadius, u.HauntWheight }).SingleAsync());
            return (result.Y, result.X, result.HauntRadius, result.HauntWheight);
        }
        public async Task<(double Latitude, double Longitude, double Radius)> GetRecentUserLocationAsync(Guid id)
        {
            var result = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new { u.CurrentLocation.Y, u.CurrentLocation.X, u.CurrentRadius }).SingleAsync());
            return (result.Y, result.X, result.CurrentRadius);
        }    

        public async Task<bool> UpdateUserAsync(Guid id, List<(string Property, object Value)> edits)
        {
            User u = new User { Id = id };
            await storeSentry.DiscussWriteAsync(ctx => ctx.Users.Attach(u));

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case "PhoneNumber":
                        u.PhoneNumber = (string)Value;
                        break;
                    case "Email":
                        u.Email = (string)Value;
                        break;
                    case "NormalisedEmail":
                        u.NormalisedEmail = (string)Value;
                        break;
                    case "Name":
                        u.Name = (string)Value;
                        break;
                    case "IsPhoneConfirmed":
                        u.IsPhoneConfirmed = (bool)Value;
                        break;
                    case "IsEmailConfirmed":
                        u.IsEmailConfirmed = (bool)Value;
                        break;
                    case "SecurityStamp":
                        u.SecurityStamp = (string)Value;
                        break;
                    case "LockoutDate":
                        u.LockoutDate = (DateTimeOffset?)Value;
                        break;
                    case "AccessTries":
                        u.AccessTries = (int)Value;
                        break;
                    case "AccountStatus":
                        u.AccountStatus = (UserAccountStatus)Value;
                        break;
                    case "Reputation":
                        u.Reputation = (int)Value;
                        break;
                    default:
                        throw new Exception("No propertyName match found");
                }
                await storeSentry.DiscussWriteAsync(ctx => ctx.Entry(u).Property(Property).IsModified = true);
            }
            await storeSentry.ExecuteWriteAsync();
            return true;
        }

        public async Task<bool> UpdateHauntAsync(Guid id, double latitude, double longitude, double radius, int stability)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateRecentLocationAsync(Guid id, double latitude, double longitude, double radius)
        {
            throw new NotImplementedException();
        }


    }
}
