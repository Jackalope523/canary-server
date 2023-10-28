using Microsoft.EntityFrameworkCore;
using Core.Boundaries;
using Xunit.Abstractions;

namespace Repository.Tests
{
    public class UserTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static AccountStore store = new AccountStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private string subjectPhoneNumber = "000-000-0000";
        private string subjectEmail = "email_0@test.com";
        private string subjectNormalizedEmail = "email_0@test.com";
        private string subjectName = "name";
        private string subjectSecurityStamp = "stamp";
        private DateTimeOffset subjectDateOfBirth = new DateTimeOffset(new DateTime(0));
        private User subject;

        public UserTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            subject = new User
            {
                PhoneNumber = subjectPhoneNumber,
                Email = subjectEmail,
                NormalizedEmail = subjectNormalizedEmail,
                Name = subjectName,
                SecurityStamp = subjectSecurityStamp,
                DateOfBirth = subjectDateOfBirth          
            };
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
        }

        /*
        [Fact]
        public void CreateUser_SUCCESS()
        {
            store.CreateUser(subjectPhoneNumber, subjectEmail, subjectName, subjectDateOfBirth);

            User created = sentry.GetContext().Users.First();

            Assert.NotNull(created);
            Assert.Equal(subjectPhoneNumber, created.PhoneNumber);
            Assert.Equal(subjectEmail, created.Email);
            Assert.Equal(subjectNormalizedEmail, created.NormalizedEmail);
            Assert.Equal(subjectName, created.Name);
            Assert.Equal(subjectDateOfBirth, created.DateOfBirth);       
        }
        */

        [Fact]
        public void DeleteUser_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));
            store.DeleteUser(subject.Id);

            int numRecords = sentry.ExecuteRead(ctx => ctx.Users.Count());

            Assert.Equal(0, numRecords);
        }

        [Fact]
        public void FindUserById_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            UserShard found = store.FindUserById(subject.Id);        

            Assert.NotNull(found);
            Assert.Equal(subject.Id, found.Id);    
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public void FindUserByPhoneNumber_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            UserShard found = store.FindUserByPhoneNumber(subjectPhoneNumber);       

            Assert.NotNull(found);
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public void FindUserByEmail_SUCCESS()
        {
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            UserShard found = store.FindUserByEmail(subjectEmail);         

            Assert.NotNull(found);
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        } 
        [Fact]
        public void UpdatePhoneNumber_SUCCESS() 
        {
            string newPhoneNumber = "111-111-1111";
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("PhoneNumber", newPhoneNumber));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.NotEqual(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(newPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public void UpdateEmail_SUCCESS() 
        {
            string newEmail = "email_1@test.com";

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Email", newEmail));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.NotEqual(subjectEmail, updated.Email);
            Assert.Equal(newEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public void UpdateNormalisedEmail_SUCCESS() 
        {
            string newNormalizedEmail = "email_1@test.com";

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("NormalisedEmail", newNormalizedEmail));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.NotEqual(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(newNormalizedEmail, updated.NormalizedEmail);          
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public void UpdateName_SUCCESS() 
        {
            string newName = "USER";

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Name", newName));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.NotEqual(subjectName, updated.Name);
            Assert.Equal(newName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public void UpdatePhoneConfirmation_SUCCESS() 
        {
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("IsPhoneConfirmed", true));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
            Assert.True(updated.IsPhoneConfirmed);
        }
        [Fact]
        public void UpdateEmailConfirmation_SUCCESS() 
        {
            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("IsEmailConfirmed", true));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
            Assert.True(updated.IsEmailConfirmed);
        }
        [Fact]
        public void UpdateSecurityStamp_SUCCESS() 
        {
            string newSecurityStamp = "STAMP";

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("SecurityStamp", newSecurityStamp));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.NotEqual(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(newSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
        }
        [Fact]
        public void UpdateLockoutDate_SUCCESS() 
        {
            DateTimeOffset newLockoutDate = new DateTimeOffset(new DateTime(1));

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("LockoutDate", newLockoutDate));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
            Assert.Equal(newLockoutDate, updated.LockoutDate);
        }
        [Fact]
        public void UpdateAccessTries_SUCCESS() 
        {
            int newAccessTries = 10;

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("AccessTries", newAccessTries));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
            Assert.Equal(newAccessTries, updated.AccessTries);
        }
        [Fact]
        public void UpdateAccountStatus_SUCCESS() 
        {
            UserAccountStatus newAccountStatus = UserAccountStatus.blacklisted;

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("AccountStatus", newAccountStatus));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
            Assert.Equal(newAccountStatus, updated.AccountStatus);
        }
        [Fact]
        public void UpdateReputation_SUCCESS() 
        {
            int newReputation = 10;

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));

            List<(string, object)> updates = new List<(string, object)>();
            updates.Add(("Reputation", newReputation));

            store.UpdateUser(subject.Id, updates);

            User updated = sentry.ExecuteRead(ctx => ctx.Users.First());

            Assert.NotNull(updated);
            Assert.Equal(subject.Id, updated.Id);
            Assert.Equal(subjectEmail, updated.Email);
            Assert.Equal(subjectNormalizedEmail, updated.NormalizedEmail);
            Assert.Equal(subjectPhoneNumber, updated.PhoneNumber);
            Assert.Equal(subjectName, updated.Name);
            Assert.Equal(subjectSecurityStamp, updated.SecurityStamp);
            Assert.Equal(subjectDateOfBirth, updated.DateOfBirth);
            Assert.Equal(newReputation, updated.Reputation);
        }


    }
}
