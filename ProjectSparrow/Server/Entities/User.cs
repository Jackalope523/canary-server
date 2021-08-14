using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Entities
{
    internal class User : Account
    {
        public string Name { get; init; }
        public DateTime DateOfBirth { get; init; }
        public Interest Interests { get; set; }

        public DateTime JoinDate { get; init; }

        public string CurrentEventID { get; private set; }

    }

    [Flags]
    internal enum Interest : ushort
    {
        None = 0,
        Sports = 1,
        Cars = 2,
        Socials = 4,
        Parties = 8,

        Everything = 0xFFFF
    }
}
