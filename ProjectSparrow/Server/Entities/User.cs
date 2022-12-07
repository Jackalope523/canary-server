using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared;

namespace Server.Entities
{
    internal class User : Account
    {
        public string Name { get; init; }
        public DateTime DateOfBirth { get; init; }
        public UserInterest Interests { get; set; }

        public DateTime JoinDate { get; init; }
        public Reputation GoerReputation { get; private set; }
        public Reputation HostReputation { get; private set; }

        public string CurrentEventID { get; private set; }
        
        public sealed override object[] Export()
        {
            object[] details = { Name, DateOfBirth, Interests, JoinDate };

            object[] information = { CurrentEventID };

            return details.Concat(information).ToArray();
        }
    }


    internal struct Reputation
    {
        public const ushort DefaultReputation = 300;

        public ushort ReputationScore { get; }
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
