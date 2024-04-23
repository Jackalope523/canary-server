using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Core.Boundaries;
using Shared;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Utilities;

namespace Frontier.Manifests
{
	////////
	// Incoming Manifests
	///////////////////////

	public class EventDetailsManifest
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventDescription { get; set; }
        
        public bool? IsOpen { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public float Radius { get; set; }

        [Required]
        public bool IsDynamic { get; set; }

        public int? GroupMinimum { get; set; }
        public int? GroupMaximum { get; set; }

        public UserSilhouette Host { get; set; }

        public uint NumberOfParticipants { get; set; }
    }

    ////////
    // Outgoing Manifests
    ///////////////////////

    public class EventManifest : Manifest
    {
        public ulong Id { get; }
        public UserSilhouetteManifest Host { get; }
        public string Name { get; }
        public string Description { get; }
        public DateTimeOffset StartTime { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public double Radius { get; }
        public DateTimeOffset? TimeEnded { get; }
        public EventState State { get; }
        public int GroupMinimum { get; }
        public int GroupMaximum { get; }
        public int NumberOfGuests { get; }

        public EventManifest(EventShard shard)
        {
            Id = shard.Id;
            Host = new(shard.Host);
            Name = shard.Name;
            Description = shard.Description;
            StartTime = shard.StartTime;
            Latitude = shard.Latitude;
            Longitude = shard.Longitude;
            Radius = shard.Radius;
            TimeEnded = shard.TimeEnded;
            State = shard.State;
            GroupMinimum = shard.GroupMinimum;
            GroupMinimum = shard.GroupMaximum;
            NumberOfGuests = shard.NumberOfGuests;
        }
    }

    public class EventHeaderManifest : Manifest
    {
        public ulong Id { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public DateTimeOffset LastActive { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public EventHeaderManifest(EventHeader header)
        {
            Id = header.Id;
            Name = header.Name;
            IsActive = header.IsActive;
            LastActive = header.LastActiveTime;
            Latitude = header.Latitude;
            Longitude = header.Longitude;
        }
    }

    public class GuestListManifest : Manifest
    {
        public int Watchers { get; set;  }
        public int GuestCount { get; set; }
        public List<(UserSilhouetteManifest, EventBond)> Guests { get; set; }
    }

    public class EtchingManifest : Manifest
    {
        public ulong Id { get; }
        public ulong EventId { get; }
        public UserSilhouetteManifest User { get; }
        public DateTimeOffset TimeEtched { get; }
        public (int Positive, int Negative) Ratings { get; }

        public EtchingManifest(Etching etching)
        {
            Id = etching.Id;
            EventId = etching.EventId;
            User = new(etching.User);
            TimeEtched = etching.TimeEtched;
            Ratings = etching.Ratings;
        }
    }
}
