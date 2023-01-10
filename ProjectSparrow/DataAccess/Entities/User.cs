
namespace DataAccess.Entities
{
    public class User : Entity
    {
        public Guid Id { get; init; }
        public string PhoneNumber { get; init; }  
        public string Passkey { get; init; }
        public string Name { get; init; }
        public DateTime DateOfBirth { get; init; }
        public DateTime JoinDate { get; init; }  
        public int Reputation { get; set; }

        internal List<Link> Links { get; set; }

    }
}
