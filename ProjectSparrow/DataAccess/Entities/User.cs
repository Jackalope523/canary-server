
using Server.Boundaries;

namespace DataAccess.Entities
{
    public class User : Entity
    {
        public Guid Id { get; init; }
        public string PhoneNumber { get; set; }  
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateOfBirth { get; init; }
        public DateTimeOffset JoinDate { get; init; }
        public int Reputation { get; set; }

        public string NormalisedEmail { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public DateTimeOffset? LockoutDate { get; set; }
        public int AccessTries { get; set; }
        public UserAccountStatus AccountStatus { get; set; }

        internal List<Link> Links { get; set; }

        public ThinUser ToThinUser()
        {
			return new(Id, PhoneNumber, Email, Name, DateOfBirth,
				IsPhoneConfirmed, IsEmailConfirmed,
				SecurityStamp, LockoutDate, AccessTries, AccountStatus,
				JoinDate, Reputation, -1);
		}
        public ThinnerUser ToThinnerUser()
        {
			return new(Id, Name);
		}
        public ThinProfile ToThinProfile()
        {
			return new(Id, Name, Reputation, 0);
		}
    }
}
