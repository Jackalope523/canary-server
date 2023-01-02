using Server.Boundaries;
using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controls
{
    internal class AccountManager
    {
        private IAccountDatabase accounts { get; init; }

        public AccountManager(IAccountDatabase accountDatabase)
        {
            accounts = accountDatabase;
        }

        public void CreateUser(string identification, string name, DateTime dateOfBirth)
        {
            // Check identification not in use

            if (accounts.FindAccount(identification) != null)
            { return; } // TODO Error

            // Create profile

            User newUser = new(identification, name, "")
            {
                AccountID = accounts.GenerateAccountID(),
                DateOfBirth = dateOfBirth,
                JoinDate = DateTime.Now,
                Verified = false
            };

            // Validate profile

            bool valid = newUser.ValidateUser();

            if (!valid)
            { return; } // TODO Add error codes, dispose of account

            // Store profile
            
            accounts.UpdateUser(newUser);
            // TODO Verify created successfully
        }

        public void EditUser(string identification)
        {
            // Verify updates are valid


        }

        public void UpdatePhoto(string identification)
        {
            Account account = accounts.FindAccount(identification);

            User user = accounts.GetUser(account.AccountID);

            accounts.UpdateUser(user);
        }

        public void DeleteUser(string identification)
        {
            Account account = accounts.FindAccount(identification);
            
            if (account == null)
            { return; } // TODO Error

            accounts.DeleteAccount(account.AccountID);
            // TODO Handle possible errors
        }

        public void FollowUser(string identification, string targetIdentification)
        {
            Account userAccount = accounts.FindAccount(identification);
            Account targetAccount = accounts.FindAccount(targetIdentification);

            if (userAccount == null || targetAccount == null)
            { return; }

            User user = accounts.GetUser(userAccount.AccountID);

            user.FollowedUserIDs.Add(targetAccount.AccountID);

            accounts.UpdateUser(user);
		}

		public void UnfollowUser(string identification, string targetIdentification)
		{
			Account userAccount = accounts.FindAccount(identification);
			Account targetAccount = accounts.FindAccount(targetIdentification);

			if (userAccount == null || targetAccount == null)
			{ return; }

			User user = accounts.GetUser(userAccount.AccountID);

			user.FollowedUserIDs.Remove(targetAccount.AccountID);

			accounts.UpdateUser(user);
		}

		public void BlockUser(string identification, string targetIdentification)
		{
			Account userAccount = accounts.FindAccount(identification);
			Account targetAccount = accounts.FindAccount(targetIdentification);

			if (userAccount == null || targetAccount == null)
			{ return; }

			User user = accounts.GetUser(userAccount.AccountID);

			user.BlockedUserIDs.Add(targetAccount.AccountID);

			accounts.UpdateUser(user);
		}

		public void UnblockUser(string identification, string targetIdentification)
		{
			Account userAccount = accounts.FindAccount(identification);
			Account targetAccount = accounts.FindAccount(targetIdentification);

			if (userAccount == null || targetAccount == null)
			{ return; }

			User user = accounts.GetUser(userAccount.AccountID);

			user.BlockedUserIDs.Remove(targetAccount.AccountID);

			accounts.UpdateUser(user);
		}
	}
}
