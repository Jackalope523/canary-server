using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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

    public class EventModel : IdentifierModel
    {
        [Required]
        public string EventID { get; set; }

        [Required]
        public EventType EventType { get; set; }

        [Required]
        public GeoLocation Location { get; set; }
    }

    public class EventOverviewModel : EventModel
    {
        public DateTime StartTime { get; set; }

        public string HostID { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

}
