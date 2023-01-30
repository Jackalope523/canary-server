using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Utilities
{
    public class GeoLocation
    {
        [Required]
        public float Latitude { get; set; }

		[Required]
		public float Longitude { get; set; }

        [Required]
        public float Distance { get; set; } = 1;
    }
}
