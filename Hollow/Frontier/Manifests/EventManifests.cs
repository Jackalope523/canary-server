using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Core.Boundaries;
using Shared;

namespace Frontier.Manifests
{
    public class EventDetailsManifest
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventDescription { get; set; }
        
        public bool? IsOpen { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public float Radius { get; set; }

        [Required]
        public bool IsDynamic { get; set; }

        public int? GroupMinimum { get; set; }
        public int? GroupMaximum { get; set; }

        public UserSilhouette Host { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

    public class EventEtchingManifest
    {
        [Required]
        public string ImageURL { get; set; }
    }
}
