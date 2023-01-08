using Server.Boundaries;
using Server.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Controls
{
    internal class AccountManager : IAccountOperations
    {
        private IAccountDatabase accounts { get; init; }

        public AccountManager(IAccountDatabase accountDatabase)
        {
            accounts = accountDatabase;
        }

        public ThinProfile GetUserProfile(string identification, string targetIdentification)
        {
            ThinAccount targetAccount = accounts.FindAccount(targetIdentification);

            if (targetAccount == null)
            { return null; ; } // TODO Error

            ThinUser targetUser = accounts.GetUser(targetAccount.AccountID);

            return new ThinProfile(targetUser.AccountID, targetUser.Name, targetUser.ProfilePhoto, targetUser.Reputation, targetUser.NumberOfFollowers);
        }

        public string TryLogin(string identification, string passkey)
        {
            // TODO Add security manager
            throw new NotImplementedException();
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
                ProfilePhoto = profilePhoto,
                Verified = false
            };

            // Validate profile

            bool valid = newUser.ValidateUser();

            if (!valid)
            { return; } // TODO Add error codes, dispose of account

            // Store profile
            
            accounts.CreateUser(newUser.Identification, passkey, newUser.Name, newUser.DateOfBirth, newUser.ProfilePhoto);
            // TODO Verify created successfully
        }

        public void EditUser(string identification, string newName, DateTime newDateOfBirth, string newPhoto)
        {
            // Verify updates are valid

            ThinAccount account = accounts.FindAccount(identification);

			if (account == null)
			{ return; } // TODO Error

			ThinUser user = accounts.GetUser(account.AccountID);

            // TODO Use only what is given

			User userValidation = new(identification, newName, "")
			{
				DateOfBirth = newDateOfBirth,
				JoinDate = DateTime.Now,
				ProfilePhoto = newPhoto,
				Verified = false
			};

			// Validate profile

			bool valid = userValidation.ValidateUser();

			if (!valid)
			{ return; } // TODO Add error codes

			accounts.UpdateUser(user.AccountID, newName, newDateOfBirth, newPhoto);
        }

        public void DeleteUser(string identification)
        {
            ThinAccount account = accounts.FindAccount(identification);
            
            if (account == null)
            { return; } // TODO Error

            accounts.DeleteAccount(account.AccountID);
            // TODO Handle possible errors
        }

        public List<ThinListUser> GetFollowedUsers(string identification)
        {
			ThinAccount account = accounts.FindAccount(identification);

			if (account == null)
			{ return null; } // TODO Error

            return accounts.GetFollowedUsers(account.AccountID);
		}

        public List<ThinListUser> GetBlockedUsers(string identification)
		{
			ThinAccount account = accounts.FindAccount(identification);

			if (account == null)
			{ return null; } // TODO Error

			return accounts.GetBlockedUsers(account.AccountID);
		}

        public void FollowUser(string identification, string targetIdentification)
        {
            ThinAccount userAccount = accounts.FindAccount(identification);
            ThinAccount targetAccount = accounts.FindAccount(targetIdentification);

            if (userAccount == null || targetAccount == null)
            { return; } // TODO Error

            accounts.FollowUser(identification, targetIdentification);
		}

		public void UnfollowUser(string identification, string targetIdentification)
		{
			ThinAccount userAccount = accounts.FindAccount(identification);
			ThinAccount targetAccount = accounts.FindAccount(targetIdentification);

			if (userAccount == null || targetAccount == null)
			{ return; } // TODO Error

			accounts.UnfollowUser(identification, targetIdentification);
		}

		public void BlockUser(string identification, string targetIdentification)
		{
			ThinAccount userAccount = accounts.FindAccount(identification);
			ThinAccount targetAccount = accounts.FindAccount(targetIdentification);

			if (userAccount == null || targetAccount == null)
			{ return; } // TODO Error

			accounts.BlockUser(identification, targetIdentification);
		}

		public void UnblockUser(string identification, string targetIdentification)
		{
			ThinAccount userAccount = accounts.FindAccount(identification);
			ThinAccount targetAccount = accounts.FindAccount(targetIdentification);

			if (userAccount == null || targetAccount == null)
			{ return; } // TODO Error

			accounts.UnblockUser(identification, targetIdentification);
		}
	}
}
