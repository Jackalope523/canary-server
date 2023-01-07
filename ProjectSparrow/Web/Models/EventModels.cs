using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    using Utilities;

    public class EventModel : IdentifierModel
    {
        [Required]
        public string EventID { get; set; }
    }

    public class EventDetailsModel : EventModel
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public GeoLocation Location { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public string HostID { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

}
