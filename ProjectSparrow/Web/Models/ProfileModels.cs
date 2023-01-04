using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ProfileModel
    {
        [Required]
        public string Identification { get; set; }

        public string TargetIdentification { get; set; }
    }

}
