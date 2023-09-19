using System;

namespace Shared
{
    [Flags]
    public enum UserInterest : ushort // ushort only allows for 15 different interests (1 - 2^16). Need to change to uint when we need more bits
    {
        None = 0,
        Sports = 1,
        Cars = 2,
        Socials = 4,
        Parties = 8,
        Educational = 16,

        Everything = 0xFFFF
    }

    public enum UserRating
    {
        Positive, Negative
    }

}
