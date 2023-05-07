namespace Repository.Entities
{
    public abstract class Link
    {
        public Guid Id { get; init; }
        public Guid SelfId { get; init; }
        internal User Self { get; init; }

        
       
    }
}