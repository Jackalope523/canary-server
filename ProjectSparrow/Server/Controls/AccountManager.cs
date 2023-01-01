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

            // Create Profile

            User newUser = new(identification, name, "")
            {
                AccountID = accounts.GenerateAccountID(),
                DateOfBirth = dateOfBirth,
                JoinDate = DateTime.Now,
                Verified = false
            };

            // Validate Profile

            bool valid = newUser.ValidateUser();

            if (!valid)
            { return; } // TODO Add error codes, dispose of account

            // Store Profile
            
            accounts.UpdateUser(newUser);
            // TODO Verify created successfully
        }

        public void EditUser(string identification)
        {
            // Verify updates are valid


        }

        public void DeleteUser(string identification)
        {
            Account account = accounts.FindAccount(identification);
            
            if (account == null)
            { return; } // TODO Error

            accounts.DeleteAccount(account.AccountID);
            // TODO Handle possible errors
        }
    }
}
