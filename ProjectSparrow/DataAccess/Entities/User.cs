using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DataAccess.Entities;

namespace DataAccess.Entities
{
    internal class User
    {
        public int Id { get; init; }
        public string Name { get; private init; }
        public DateTime DateOfBirth { get; init; }
        public DateTime JoinDate { get; init; }  
        public bool Verified { get; set; }
        public int Reputation { get; set; }

        internal List<Link> Links { get; set; }

        
      
        

       
    }
}
