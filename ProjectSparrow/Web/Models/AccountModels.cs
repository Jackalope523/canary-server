using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{

    public class TargetModel
    {
        [Required]
        public Guid TargetID { get; set; }
    }

    public class AccountCredentialsModel
    {
		[Required]
		public string PhoneNumber { get; set; }

		public string Code { get; set; }
    }

    public class AccountSignUpModel
	{
		[Required]
		public string PhoneNumber { get; set; }

		public string Email { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime DateOfBirth { get; set; }
    }

	public class AccountDetailsModel
	{
		public string Name { get; set; }
	}

}
