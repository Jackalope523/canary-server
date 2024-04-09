using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Manifests
{
	////////
	// Incoming Manifests
	///////////////////////
	
    public class TargetManifest
    {
        [Required]
        public ulong TargetId { get; set; }
    }

    public class AccountCredentialsManifest
    {
		[Required]
		public string PhoneNumber { get; set; }

		public string Code { get; set; }
    }

    public class AccountSignUpManifest
	{
		[Required]
		public string PhoneNumber { get; set; }

		public string Email { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime DateOfBirth { get; set; }
    }

	////////
	// Outgoing Manifests
	///////////////////////

	public class UserManifest
	{
		public ulong Id { get; }
	}
}
