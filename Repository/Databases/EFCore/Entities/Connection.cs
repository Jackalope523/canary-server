namespace Repository
{
    internal class Connection : Entity
    {
        public long UserId { get; set; }

        // Navigation Properties
        public User? User { get; set; }
    }
}
