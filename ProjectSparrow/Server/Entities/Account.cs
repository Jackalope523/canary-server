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
        public string UniqueID { get; }
        public string Identification { get; }

        private readonly string passkey;

        protected Account()
        {

        }

        public bool VerifyPasskey(string otherPasskey)
        {
            bool isValid = true;

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
