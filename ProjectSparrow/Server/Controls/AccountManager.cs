using Server.Boundaries;
using Server.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controls
{
    internal class AccountManager : IAccountOperations
    {
        private IAccountDatabase accounts { get; init; }

        public AccountManager(IAccountDatabase accountDatabase)
        {
            accounts = accountDatabase;
        }

        public ThinUser GetUserProfile(string identification, string targetIdentification)
        {
            // Cases to handle
            // Is my own profile (send everything not hidden, include number of followers) 
            // Is someone else's profile (send everything public)
            //
        }

        public string TryLogin(string identification, string passkey)
        {

        }

        public void CreateUser(string identification, string passkey, string name, DateTime dateOfBirth, string profilePhoto)
        {
            // Check identification not in use

            if (accounts.FindAccount(identification) != null)
            { return; } // TODO Error

            // Create profile

            User newUser = new(identification, name, "")
            {
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

        public void EditUser(string identification, string newName, DateTime newDateOfBirth, string newPhoto)
        {
            // Verify updates are valid

            ThinAccount account = accounts.FindAccount(identification);

			if (account == null)
			{ return; } // TODO Error

			ThinUser user = accounts.GetUser(account.AccountID);

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

        public List<ThinListUser> GetFollowedUsers(string identification)
        {
			Account account = accounts.FindAccount(identification);

			if (account == null)
			{ return; } // TODO Error

			User user = accounts.GetUser(account.AccountID);

            user.FollowedUserIDs.ToImmutableHashSet(); // TODO Return contents
		}

        public List<ThinListUser> GetBlockedUsers(string identification)
		{
			Account account = accounts.FindAccount(identification);

			if (account == null)
			{ return; } // TODO Error

			User user = accounts.GetUser(account.AccountID);

			user.BlockedUserIDs.ToImmutableHashSet(); // TODO Return contents
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
