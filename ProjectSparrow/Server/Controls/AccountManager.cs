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
        public void CreateUser(string identification, string name, DateTime dateOfBirth)
        {
            // Create Profile

            User newUser = new(identification, name, "")
            {
                DateOfBirth = dateOfBirth,
                JoinDate = DateTime.Now,
                Verified = false
            };

            // Validate Profile

            bool valid = newUser.ValidateUser();

            if (!valid) { return; } // TODO Add error codes, dispose of account

            // Store Profile
            // TODO
        }
    }
}
