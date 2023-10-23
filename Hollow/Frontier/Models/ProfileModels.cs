using System;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace Frontier.Models
{
    public class AccountDetailsModel
    {
        public string Name { get; set; }
    }

    public class AccountRatingModel
    {
        [Required]
        public UserRating Rating { get; set; }
    }
}

