namespace DataAccess.Entities
{
    public abstract class Link
    {
        public Guid Id { get; init; }
        public int SelfId { get; init; }
        internal User Self { get; init; }

        
       
    }
}