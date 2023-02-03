using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Server.Boundaries;

namespace Web.Models
{
    public class EventModel
    {
        [Required]
        public Guid EventID { get; set; }
    }

    public class EventDetailsModel : EventModel
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public ThinnerUser Host { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

}
