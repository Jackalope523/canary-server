using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared;

namespace Server.Entities
{
    internal class User : Account
    {
        public string Name { get; private init; }
        public DateTime DateOfBirth { get; init; }
        public UserInterest Interests { get; set; }
        public string ProfilePhoto { get; set; } = "none";

        public DateTime JoinDate { get; init; }
        public Reputation GoerReputation { get; private set; }
        public Reputation HostReputation { get; private set; }

        public HashSet<string> FollowedUserIDs { get; init; }
        public HashSet<string> BlockedUserIDs { get; init; }

        public string CurrentEventID { get; private set; }

        public bool Verified { get; set; }

        public User(string identification, string name, string passkey) : base(identification, passkey)
        {
            Name = name;
            GoerReputation = new Reputation();
            HostReputation = new Reputation();
        }

        public bool ValidateUser()
        {
            if (!ValidateAccount()) { return false; }

            // Check Name is suitable

            // Verify DateOfBirth

            return true;
        }
    }


    internal struct Reputation
    {
        public const ushort DefaultReputation = 300;

        public ushort ReputationScore { get; set; }
        public ReputationTier ReputationTier 
        {
            get
            {
                var tiers = Enum.GetValues<ReputationTier>().Reverse();

                foreach (ReputationTier tier in tiers)
                {
                    if (ReputationScore >= (ushort)tier)
                    {
                        return tier;
                    }
                }

                return ReputationTier.Poor;
            }
        }

        public Reputation() { ReputationScore = DefaultReputation; }

    }


    internal enum ReputationTier : ushort
    {
        Poor = 0,
        Mediocre = 200,
        Normal = 400,
		Good = 600,
        Great = 800,
        Excellent = 900
    }


}
