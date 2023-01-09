namespace DataAccess.Entities
{
    public abstract class Link : Entity
    {
        public Guid Id { get; init; }
        public Guid SelfId { get; init; }
        internal User Self { get; init; }

        
       
    }
}