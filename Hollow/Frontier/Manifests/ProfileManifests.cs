using System;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace Frontier.Manifests
{
	////////
	// Incoming Manifests
	///////////////////////

	public class AccountDetailsManifest
    {
        public string Name { get; set; }
    }

    public class AccountRatingManifest
    {
        [Required]
        public UserRating Rating { get; set; }
    }
}

