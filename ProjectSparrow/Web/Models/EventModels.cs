using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    using Utilities;

    public enum EventType
    {
        // Might want this to be a String, goes back to Colt Express enum converting. Drawbacks will appear though, especially with scaling
    }

    public enum EventStatus
    {
        //
    }

    public class EventModel
    {
        public string EventUUID { get; set; }

        public EventType EventType { get; set; }

        public GeoLocation Location { get; set; }
    
        public EventStatus EventStatus { get; set; }
    }

    public class EventOverviewModel : EventModel
    {
        public DateTime StartTime { get; set; }

        public string HostUUID { get; set; } // May be better if we don't give out the Host's UUID and instead just give relevant information or have calls that use the Event's UUID instead

        public uint NumberOfParticipants { get; set; }
    }

}
