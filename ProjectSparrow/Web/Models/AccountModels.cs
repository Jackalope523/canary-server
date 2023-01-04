using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
	public class IdentifierModel
	{
		[Required]
		public string Identification { get; set; }
	}

    public class AccountCredentialsModel : IdentifierModel
    {
        [Required]
        public string Passkey { get; set; }
    }

    public class AccountSignUpModel : IdentifierModel
	{
		[Required]
		public string Passkey { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime DateOfBirth { get; set; }

		public string Photo { get; set; }
    }

	public class AccountDetailsModel : IdentifierModel
	{
		public string Name { get; set; }

		public DateTime DateOfBirth { get; set; }

		public string Photo { get; set; }
	}

}
