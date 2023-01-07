namespace DataAccess.Entities
{
    internal abstract class Link
    {
        public int Id { get; init; }
        public int SelfId { get; init; }
        internal User Self { get; init; }

        
       
    }
}