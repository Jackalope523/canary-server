using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities
{
    public class Event
    {
        public Guid Id { get; set; }

        internal string Name { get; set; }
        internal DateTime StartTime { get; set; }
        internal int HostId {  get; set; }
        internal User Host { get; set; }

        internal float Latitude { get; set; }
        internal float Longitude { get; set; }  
         

        internal List<EventLink> Links { get; set; }       
    }
}
