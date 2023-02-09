using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Server.Boundaries;
using Shared;

namespace Web.Models
{
    public class EventDetailsModel
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventDescription { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        public int? GroupMinimum { get; set; }
        public int? GroupMaximum { get; set; }

        public ThinnerUser Host { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

    public class EventEditModel
    {
        public string EventDescription { get; set; }
        public string EventType { get; set; }
        public bool? EventIsOpen { get; set; }
    }

    public class EventReportModel
    {
        [Required]
        public EventReport ReportType { get; set; }

        public string ReportDetails { get; set; }
    }
}
