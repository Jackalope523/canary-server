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
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        public int? GroupMinimum { get; set; }
        public int? GroupMaximum { get; set; }

        public UserSilhouette Host { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

    public class EventEditModel
    {
        public string EventDescription { get; set; }
        public bool? EventIsOpen { get; set; }
    }

    public class EventReportModel
    {
        [Required]
        public EventReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }

    public class EventPostModel
    {
        [Required]
        public string ImageURL { get; set; }
    }

    public class FeedModel
    {
        [Required]
        public int Depth { get; set; }

        [Required]
        public Guid[] ExclusionList { get; set; }
    }
}
