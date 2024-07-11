using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Serilog;

namespace Repository
{
    public class EFCoreAccountStore : QueryStore, IAccountDatabase
    {
        public EFCoreAccountStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task CreateUserAsync(string phoneNumber, string email, string normalisedEmail, string name, DateTimeOffset dateOfBirth, DateTimeOffset joinDate, Character character)
        {
            Log.Error(phoneNumber);

            User toCreate = new()
            {
                PhoneNumber = phoneNumber,
                Email = email,
                NormalisedEmail = normalisedEmail,
                Name = name,
                DateOfBirth = dateOfBirth,
                JoinDate = joinDate,
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,              
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Users.Add(toCreate));
        }

        public async Task DeleteUserAsync(ulong id)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == id).
                ExecuteUpdate(setter => setter.SetProperty(u => u.IsPendingDeletion, true)));

            List<ulong> upcomingGatherings = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Gatherings.
                Where(e => e.HostId == id && e.State == GatheringState.Upcoming).
                Select(e => e.Id).
                ToListAsync());

            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Gatherings.
               Where(e => upcomingGatherings.Contains(e.Id)).
               ExecuteUpdate(setter => setter.SetProperty(e => e.IsPendingDeletion, true)));
        
            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Notes.
               Where(n => n.NotifierId == id || n.RecipientId == id).
               ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Snapshots.
               Where(p => p.OwnerId == id).
               ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Subscriptions.
               Where(s => s.UserId == id).
               ExecuteDelete());
        }

        public async Task<CoreUser> FindUserByIdAsync(ulong id) 
        {            
            int numAppreciateers;
            CoreUser user;
            try 
            {
               user = await storeSentry.ExecuteReadAsync(ctx => 
               ctx.Users.
               Where(u => u.Id == id).
               Select(u => new CoreUser
               (
                   u.Id,
                   u.PhoneNumber,
                   u.Email,
                   u.Name,
                   u.DateOfBirth,
                   u.IsPhoneConfirmed,
                   u.IsEmailConfirmed,
                   u.IsPendingDeletion,
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
            
            numAppreciateers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Appreciate).CountAsync());

            return user with { Appreciation = numAppreciateers };
        }
        public async Task<CoreUser> FindUserByPhoneNumberAsync(string phoneNumber) 
        {
            Log.Error(phoneNumber);

            int numAppreciateers;
            CoreUser user;
            try
            {
              user = await storeSentry.ExecuteReadAsync(ctx => 
              ctx.Users.
              Where(u => u.PhoneNumber == phoneNumber).
              Select(u => new CoreUser
              (
                  u.Id,
                  u.PhoneNumber,
                  u.Email,
                  u.Name,
                  u.DateOfBirth,
                  u.IsPhoneConfirmed,
                  u.IsEmailConfirmed,
                  u.IsPendingDeletion,
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
         
            numAppreciateers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Appreciate).CountAsync());

            return user with { Appreciation = numAppreciateers };
        }
        public async Task<CoreUser> FindUserByEmailAsync(string email) 
        { 
            int numAppreciateers;
            CoreUser user;
            try
            {
              user = await storeSentry.ExecuteReadAsync(ctx => 
              ctx.Users.
              Where(u => u.Email == email).
              Select(u => new CoreUser
              (
                  u.Id,
                  u.PhoneNumber,
                  u.Email,
                  u.Name,
                  u.DateOfBirth,
                  u.IsPhoneConfirmed,
                  u.IsEmailConfirmed,
                  u.IsPendingDeletion,
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

            numAppreciateers = await storeSentry.ExecuteReadAsync(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Appreciate).CountAsync());

            return user with { Appreciation = numAppreciateers };
        }

        public async Task<Haunt> GetUserHauntAsync(ulong id)
        {
            try
            {
                return await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.Id == id).
                Select(u => new Haunt(u.Haunt.Y, u.Haunt.X, u.HauntRadius, u.HauntWheight)).
                SingleAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }
        }
        public async Task<RecentLocation> GetRecentUserLocationAsync(ulong id)
        {       
            try
            {
                return await storeSentry.ExecuteReadAsync(ctx =>
                            ctx.Users.
                            Where(u => u.Id == id).
                            Select(u => new RecentLocation(u.CurrentLocation.Y, u.CurrentLocation.X, u.CurrentRadius)).
                            SingleAsync());

            }
            catch (InvalidOperationException ex)
            {
                throw new UserNotFoundException("Unable to find a user bearing supplied Id.", ex);
            }                 
        }    

        public async Task UpdateUserAsync(ulong id, List<(string Property, object Value)> edits)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            User u = new() { Id = id };

            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u), currentDiscussion);

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
                storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(Property).IsModified = true, currentDiscussion);
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }

        public async Task UpdateHauntAsync(ulong id, double latitude, double longitude, double radius, int stability)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Point newHaunt = new CoordinateFactory().Create(longitude, latitude);
            User u = new() { Id = id, Haunt = newHaunt , HauntRadius = radius, HauntWheight = stability };

            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u), currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.Haunt)).IsModified = true, currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.HauntRadius)).IsModified = true, currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.HauntWheight)).IsModified = true, currentDiscussion);
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }

        public async Task UpdateRecentLocationAsync(ulong id, double latitude, double longitude, double radius)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Point newCurrentLocation = new CoordinateFactory().Create(longitude, latitude);
            User u = new() { Id = id, CurrentLocation = newCurrentLocation, CurrentRadius = radius };

            storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u), currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.CurrentLocation)).IsModified = true, currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.CurrentRadius)).IsModified = true, currentDiscussion);
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }
    }
}
