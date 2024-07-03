using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Core.Boundaries;

using Microsoft.Extensions.Hosting;
using NetTopologySuite.Utilities;

namespace Frontier.Manifests
{
	public class GatheringDetailsManifest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        
        public bool? IsOpen { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public string FriendlyLocation { get; set; }

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
}
