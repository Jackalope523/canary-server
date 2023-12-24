using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Shared;

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
                JoinDate = DateTimeOffset.Now,
                Reputation = 50,
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,              
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.Add(toCreate));
            return true;
        }

        public async Task<bool> DeleteUserAsync(ulong id)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.Remove(new User { Id = id }));
            return true;
        }

        public async Task<UserShard> FindUserByIdAsync(ulong id) 
        {            
            int numFollowers;
            UserShard user;
            try 
            {
               user = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new UserShard
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
            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }
            
            numFollowers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).CountAsync());

            return user with { NumberOfFollowers = numFollowers };
        }
        public async Task<UserShard> FindUserByPhoneNumberAsync(string phoneNumber) 
        { 
            int numFollowers;
            UserShard user;
            try
            {
              user = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.PhoneNumber == phoneNumber).Select(u => new UserShard
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
            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }
         
            numFollowers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).CountAsync());

            return user with { NumberOfFollowers = numFollowers };
        }
        public async Task<UserShard> FindUserByEmailAsync(string email) 
        { 
            int numFollowers;
            UserShard user;
            try
            {
              user = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Email == email).Select(u => new UserShard
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
            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }

            numFollowers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).CountAsync());

            return user with { NumberOfFollowers = numFollowers };
        }

        public async Task<(double Latitude, double Longitude, double Radius, int Stability)> GetUserHauntAsync(ulong id)
        {
            try
            {
                var result = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == id).Select(u => new { u.Haunt.Y, u.Haunt.X, u.HauntRadius, u.HauntWheight }).SingleAsync());
                return (result.Y, result.X, result.HauntRadius, result.HauntWheight);
            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }
        }
        public async Task<(double Latitude, double Longitude, double Radius)> GetRecentUserLocationAsync(ulong id)
        {       
            try
            {
                var result = await storeSentry.ExecuteReadAsync(ctx =>
                            ctx.Users.
                            Where(u => u.Id == id).
                            Select(u => new { u.CurrentLocation.Y, u.CurrentLocation.X, u.CurrentRadius }).
                            SingleAsync());

                return (result.Y, result.X, result.CurrentRadius);
            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }                 
        }    

        public async Task<bool> UpdateUserAsync(ulong id, List<(string Property, object Value)> edits)
        {                   
            User u = new User { Id = id };

            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u));

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
                        throw new InvalidInputException("Property named \"" + Property + "\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(Property).IsModified = true);
            }
            await storeSentry.ExecuteWriteAsync();
            return true;
        }

        public async Task<bool> UpdateHauntAsync(ulong id, double latitude, double longitude, double radius, int stability)
        {
            User u = new User { Id = id, Haunt = new Point(longitude, latitude) , HauntRadius = radius, HauntWheight = stability };

            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u));
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.Haunt)).IsModified = true);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.HauntRadius)).IsModified = true);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.HauntWheight)).IsModified = true);
            await storeSentry.ExecuteWriteAsync();

            return true;
        }

        public async Task<bool> UpdateRecentLocationAsync(ulong id, double latitude, double longitude, double radius)
        {
            User u = new User { Id = id, CurrentLocation = new Point(longitude, latitude), CurrentRadius = radius };

            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u));
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.CurrentLocation)).IsModified = true);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.CurrentRadius)).IsModified = true);
            await storeSentry.ExecuteWriteAsync();

            return true;
        }


    }
}
