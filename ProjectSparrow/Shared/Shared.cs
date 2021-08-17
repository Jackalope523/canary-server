using System;

namespace Shared
{
    [Flags]
    public enum UserInterest : ushort
    {
        None = 0,
        Sports = 1,
        Cars = 2,
        Socials = 4,
        Parties = 8,

        Everything = 0xFFFF
    }

}
