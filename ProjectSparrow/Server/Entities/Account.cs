using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Server.Entities
{
    internal abstract class Account
    {
        public Guid AccountID { get; init; }
        public string Identification { get; init; }


        protected Account(string identification)
        { }

        /*public bool VerifyPasskey(string otherPasskey)
        {
            bool isValid = otherPasskey.Length == accountPasskey.Length;

            for (int i = 0; i < otherPasskey.Length && i < accountPasskey.Length; i++)
            {
                if (otherPasskey[i] != accountPasskey[i])
                {
                    isValid = false;
                }
            }

            return isValid;
        }*/

        public bool ValidateAccount()
        {
            // Check Identification is suitable

            // Check Passkey complies

            return true;
        }

    }
}
