using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Repository.Contexts;
using Repository.Entities;
using Repository.Sentries;
using Server.Boundaries;
using Shared;
using System;
using System.Numerics;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static PhoneNumbers.PhoneNumber;

namespace Repository.Tests
{
    public class UserTests
    {
        private static TestSentry sentry = TestSentry.GetTestSentry();
        private static QueryStore store = new QueryStore(sentry);

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

        [Fact]
        public void CreateUser_SUCCESS()
        {
            store.CreateUser(subjectPhoneNumber, subjectEmail, subjectName, subjectDateOfBirth);

            User created = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(created);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(created);
            Assert.Equal(subjectPhoneNumber, created.PhoneNumber);
            Assert.Equal(subjectEmail, created.Email);
            Assert.Equal(subjectNormalizedEmail, created.NormalizedEmail);
            Assert.Equal(subjectName, created.Name);
            Assert.Equal(subjectDateOfBirth, created.DateOfBirth);       
        }

        [Fact]
        public void DeleteUser_SUCCESS()
        {
            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;

            store.DeleteUser(id);

            int numRecords = sentry.GetContext().Users.Count();

            Assert.Equal(0, numRecords);
        }

        [Fact]
        public void FindUserById_SUCCESS()
        {
            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            ThinUser found = store.FindUserById(id);

            sentry.GetContext().Users.Remove(subject);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(found);
            Assert.Equal(id, found.Id);    
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public void FindUserByPhoneNumber_SUCCESS()
        {
            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            ThinUser found = store.FindUserByPhoneNumber(subjectPhoneNumber);

            sentry.GetContext().Users.Remove(subject);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(found);
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public void FindUserByEmail_SUCCESS()
        {
            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            ThinUser found = store.FindUserByEmail(subjectEmail);

            sentry.GetContext().Users.Remove(subject);
            sentry.GetContext().SaveChanges();

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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdatePhoneNumber(id, newPhoneNumber);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateEmail(id, newEmail);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateNormalisedEmail(id, newNormalizedEmail);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateName(id, newName);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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
            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdatePhoneConfirmation(id, true);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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
            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateEmailConfirmation(id, true);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateSecurityStamp(id, newSecurityStamp);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateLockoutDate(id, newLockoutDate);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateAccessTries(id, newAccessTries);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateAccountStatus(id, newAccountStatus);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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

            sentry.GetContext().Users.Add(subject);
            sentry.GetContext().SaveChanges();

            Guid id = sentry.GetContext().Users.First().Id;
            store.UpdateReputation(id, newReputation);

            sentry.GetContext();

            User updated = sentry.GetContext().Users.First();

            sentry.GetContext().Users.Remove(updated);
            sentry.GetContext().SaveChanges();

            Assert.NotNull(updated);
            Assert.Equal(id, updated.Id);
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
