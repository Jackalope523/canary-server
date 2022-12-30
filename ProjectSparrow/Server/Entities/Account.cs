using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Server.Entities
{
    internal abstract class Account
    {
        public string AccountID { get; }
        public string Identification { get; }

        private readonly string passkey; // TODO Passkey may change. Create new instance? Change from readonly?

        protected Account()
        {

        }

        public bool VerifyPasskey(string otherPasskey)
        {
            bool isValid = otherPasskey.Length == passkey.Length;

            for (int i = 0; i < otherPasskey.Length && i < passkey.Length; i++)
            {
                if (otherPasskey[i] != passkey[i])
                {
                    isValid = false;
                }
            }

            return isValid;
        }

    }
}
