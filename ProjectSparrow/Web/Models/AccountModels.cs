using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{

    public enum EventInterest
    {
        
    }

    public class AccountCredentialsModel
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class AccountDetailsModel
    {
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Language { get; set; }

        public EventInterest[] Interests { get; set; }

    }

}
