using Shared;
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

	public class AccountReportModel
	{
		[Required]
		public UserReportType ReportType { get; set; }

		public string ReportDetails { get; set; }
	}

	public class AccountRatingModel
	{
		[Required]
		public UserRating Rating { get; set; }
	}
}
