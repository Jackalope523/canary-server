using Microsoft.EntityFrameworkCore;
using Core.Boundaries;
using Xunit.Abstractions;

namespace Repository.Tests
{
    public class AccountStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static AccountStore store = new AccountStore(sentry);
        private static UserFactory userFactory = new UserFactory();

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject ;

        public AccountStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            subject = userFactory.Create();
        }
        public void Dispose()
        {
            sentry.ExecuteWriteAsync(ctx => ctx.Users.ExecuteDelete());
        }

        
        [Fact]
        public async Task CreateUserAsync_SUCCESS()
        {
            await store.CreateUserAsync(
                subject.PhoneNumber, 
                subject.Email, 
                subject.NormalisedEmail,
                subject.Name, 
                subject.DateOfBirth,
                new Character(
                    subject.Extroversion,
                    subject.Athleticisme,
                    subject.Chaos,
                    subject.Competitiveness,
                    subject.Industriousness,
                    subject.NightOwl,
                    subject.Openness
                    ));

            User created = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(created);
            Assert.Equal(subject.PhoneNumber, created.PhoneNumber);
            Assert.Equal(subject.Email, created.Email);
            Assert.Equal(subject.NormalisedEmail, created.NormalisedEmail);
            Assert.Equal(subject.Name, created.Name);
            Assert.Equal(subject.DateOfBirth, created.DateOfBirth);       
        }  

        [Fact]
        public async Task DeleteUserAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));
            await store.DeleteUserAsync(subject.Id);

            int numRecords = await sentry.ExecuteReadAsync(ctx => ctx.Users.CountAsync());

            Assert.Equal(0, numRecords);
        }

        [Fact]
        public async Task FindUserByIdAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            UserShard found = await store.FindUserByIdAsync(subject.Id);        

            Assert.NotNull(found);
            Assert.Equal(subject.Id, found.Id);    
            Assert.Equal(subject.PhoneNumber, found.PhoneNumber);
            Assert.Equal(subject.Email, found.Email);
            Assert.Equal(subject.Name, found.Name);
            Assert.Equal(subject.DateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public async Task FindUserByPhoneNumberAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            UserShard found = await store.FindUserByPhoneNumberAsync(subject.PhoneNumber);       

            Assert.NotNull(found);
            Assert.Equal(subject.PhoneNumber, found.PhoneNumber);
            Assert.Equal(subject.Email, found.Email);
            Assert.Equal(subject.Name, found.Name);
            Assert.Equal(subject.DateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public async Task FindUserByEmailAsync_SUCCESS()
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            UserShard found = await store.FindUserByEmailAsync(subject.Email);         

            Assert.NotNull(found);
            Assert.Equal(subject.PhoneNumber, found.PhoneNumber);
            Assert.Equal(subject.Email, found.Email);
            Assert.Equal(subject.Name, found.Name);
            Assert.Equal(subject.DateOfBirth, found.DateOfBirth);
        } 
        [Fact]
        public async Task UpdateUserAsync_PhoneNumber() 
        {
            string newPhoneNumber = "111-111-1111";
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("PhoneNumber", newPhoneNumber));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.NotEqual(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(newPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public async Task UpdateUserAsync_Email() 
        {
            string newEmail = "email_1@test.com";

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Email", newEmail));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.NotEqual(subject.Email, updated.Email);
            Assert.Equal(newEmail, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public async Task UpdateUserAsync_NormalisedEmailS() 
        {
            string newNormalizedEmail = "email_1@test.com";

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("NormalisedEmail", newNormalizedEmail));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.NotEqual(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(newNormalizedEmail, updated.NormalisedEmail);          
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public async Task UpdateUserAsync_Name() 
        {
            string newName = "USER";

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Name", newName));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.NotEqual(subject.Name, updated.Name);
            Assert.Equal(newName, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public async Task UpdateUserAsync_PhoneConfirmation() 
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("IsPhoneConfirmed", true));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
            Assert.True(updated.IsPhoneConfirmed);
        }
        [Fact]
        public async Task UpdateUserAsync_EmailConfirmation() 
        {
            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("IsEmailConfirmed", true));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
            Assert.True(updated.IsEmailConfirmed);
        }
        [Fact]
        public async Task UpdateUserAsync_SecurityStamp() 
        {
            string newSecurityStamp = "STAMP";

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("SecurityStamp", newSecurityStamp));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.NotEqual(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(newSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public async Task UpdateUserAsync_LockoutDate() 
        {
            DateTimeOffset newLockoutDate = new DateTimeOffset(new DateTime(1));

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("LockoutDate", newLockoutDate));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
            Assert.Equal(newLockoutDate, updated.LockoutDate);
        }
        [Fact]
        public async Task UpdateUserAsync_AccessTries() 
        {
            int newAccessTries = 10;

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("AccessTries", newAccessTries));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
            Assert.Equal(newAccessTries, updated.AccessTries);
        }
        [Fact]
        public async Task UpdateUserAsync_AccountStatus() 
        {
            UserAccountStatus newAccountStatus = UserAccountStatus.blacklisted;

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("AccountStatus", newAccountStatus));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
            Assert.Equal(newAccountStatus, updated.AccountStatus);
        }
        [Fact]
        public async Task UpdateUserAsync_Reputation() 
        {
            int newReputation = 10;

            await sentry.ExecuteWriteAsync(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Reputation", newReputation));

            await store.UpdateUserAsync(subject.Id, updates);

            User updated = await sentry.ExecuteReadAsync(ctx => ctx.Users.FirstAsync());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subject.Email, updated.Email);
            Assert.Equal(subject.NormalisedEmail, updated.NormalisedEmail);
            Assert.Equal(subject.PhoneNumber, updated.PhoneNumber);
            Assert.Equal(subject.Name, updated.Name);
            Assert.Equal(subject.SecurityStamp, updated.SecurityStamp);
            Assert.Equal(subject.DateOfBirth, updated.DateOfBirth);
            Assert.Equal(newReputation, updated.Reputation);
        }
        [Fact]
        public async Task GetUserHauntAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task GetRecentUserLocationAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task UpdateHauntAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public async Task UpdateRecentLocationAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}
