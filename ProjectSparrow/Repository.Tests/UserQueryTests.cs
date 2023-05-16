using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Repository.Contexts;
using Repository.Entities;
using Server.Boundaries;
using Shared;
using System;
using System.Numerics;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static PhoneNumbers.PhoneNumber;

namespace Repository.Tests
{
    public class UserQueryTests
    {
        private static TestContext _context = new TestContext();
        private static QueryStore store = new QueryStore(QueryStore.StoreMode.Test, 1);

        private readonly ITestOutputHelper _testOutputHelper;

        private string subjectPhoneNumber = "000-000-0000";
        private string subjectEmail = "email_0@test.com";
        private string subjectNormalizedEmail = "email_0@test.com";
        private string subjectName = "name";
        private string subjectSecurityStamp = "stamp";
        private DateTimeOffset subjectDateOfBirth = new DateTimeOffset(new DateTime(0));
        private User subject;

        public UserQueryTests(ITestOutputHelper testOutputHelper)
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

            User created = _context.Users.First();

            _context.Users.Remove(created);
            _context.SaveChanges();

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
            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;

            store.DeleteUser(id);

            int numRecords = _context.Users.Count();

            Assert.Equal(0, numRecords);
        }

        [Fact]
        public void FindUserById_SUCCESS()
        {
            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            ThinUser found = store.FindUserById(id);

            _context.Users.Remove(subject);
            _context.SaveChanges();

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
            _context.Users.Add(subject);
            _context.SaveChanges();

            ThinUser found = store.FindUserByPhoneNumber(subjectPhoneNumber);

            _context.Users.Remove(subject);
            _context.SaveChanges();

            Assert.NotNull(found);
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        }
        [Fact]
        public void FindUserByEmail_SUCCESS()
        {
            _context.Users.Add(subject);
            _context.SaveChanges();

            ThinUser found = store.FindUserByEmail(subjectEmail);

            _context.Users.Remove(subject);
            _context.SaveChanges();

            Assert.NotNull(found);
            Assert.Equal(subjectPhoneNumber, found.PhoneNumber);
            Assert.Equal(subjectEmail, found.Email);
            Assert.Equal(subjectName, found.Name);
            Assert.Equal(subjectDateOfBirth, found.DateOfBirth);
        }

        [Fact]
        public void FollowUser_SUCCESS()
        {

        }
        [Fact]
        public void UnfollowUser_SUCCESS()
        {

        }
        [Fact]
        public void BlockUser_SUCCESS()
        {

        }
        [Fact]
        public void UnblockUser_SUCCESS()
        {

        }
        [Fact]
        public void RateUser_SUCCESS()
        {

        }

        [Fact]
        public void UpdatePhoneNumber_SUCCESS() 
        {
            string newPhoneNumber = "111-111-1111";

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdatePhoneNumber(id, newPhoneNumber);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateEmail(id, newEmail);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateNormalisedEmail(id, newNormalizedEmail);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateName(id, newName);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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
            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdatePhoneConfirmation(id, true);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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
            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateEmailConfirmation(id, true);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateSecurityStamp(id, newSecurityStamp);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateLockoutDate(id, newLockoutDate);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateAccessTries(id, newAccessTries);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateAccountStatus(id, newAccountStatus);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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

            _context.Users.Add(subject);
            _context.SaveChanges();

            Guid id = _context.Users.First().Id;
            store.UpdateReputation(id, newReputation);

            _context.Dispose();
            _context = new TestContext();

            User updated = _context.Users.First();

            _context.Users.Remove(updated);
            _context.SaveChanges();

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
