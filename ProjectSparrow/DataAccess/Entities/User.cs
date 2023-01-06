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
        public int GoerReputation { get; set; }
        public int HostReputation { get; set; }

        private List<Link> Links;

        public User(int id, string name, DateTime dateOfBirth, DateTime joinDate, bool verified, int goerReputation, int hostReputation, List<Link> links)
        {
            Id = id;
            Name = name;
            DateOfBirth = dateOfBirth;
            JoinDate = joinDate;
            Verified = verified;
            GoerReputation = goerReputation;
            HostReputation = hostReputation;
            Links = links;
        }

        public void AddLink(int selfId, User self, int otherId, User other, Link.UserLinkType type)
        {
            Links.Add(new UserLink(selfId, self, otherId, other, type));
        }

        public void AddLink(int userId, User user, int eventId, Event @event, Link.EventLinkType type)
        {
            Links.Add(new EventLink(userId, user, eventId, @event, type));
        }
    }
}
