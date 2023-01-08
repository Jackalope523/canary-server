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
		public Guid UserID { get; set; }
	}

    public class TargetModel : IdentifierModel
    {
        [Required]
        public Guid TargetID { get; set; }
    }

    public class AccountCredentialsModel
    {
		[Required]
		public string PhoneNumber { get; set; }

        [Required]
        public string Passkey { get; set; }
    }

    public class AccountSignUpModel
	{
		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public string Passkey { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime DateOfBirth { get; set; }
    }

	public class AccountDetailsModel : IdentifierModel
	{
		public string PhoneNumber { get; set; }

		public string Name { get; set; }
	}

}
