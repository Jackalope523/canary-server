using System;
using Shared;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Core.Boundaries;

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

    ////////
    // Outgoing Manifests
    ///////////////////////

    public class UserProfileManifest
    {
        public ulong Id { get; }
        public string Name { get; }
        public int Reputation { get; }
        public int NumberOfFollowers { get; }

        public UserProfileManifest(UserProfile profile)
        {
            Id = profile.Id;
            Name = profile.Name;
            Reputation = profile.Reputation;
            NumberOfFollowers = profile.NumberOfFollowers;
        }
    }

    public class UserSilhouetteManifest
    {
        public ulong Id { get; }
        public string Name { get; }

        public UserSilhouetteManifest(UserSilhouette silhouette)
        {
            Id = silhouette.Id;
            Name = silhouette.Name;
        }
    }

    public class NestManifest
    {
        public List<EventManifest> Events { get; set; }
        public List<EtchingManifest> Etchings { get; set; }
    }

    public class FriendActivityManifest
    {
        public IDictionary<UserSilhouetteManifest, List<EventManifest>> Activity { get; set; }
    }
}

