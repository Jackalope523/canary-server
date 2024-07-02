using Core.Boundaries;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using static Core.Entities.Arbiter;
using static Core.Entities.Artificer;
using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class GatheringDirector : AbstractDirector, IGatheringOperations
	{
		#region Initialisation

		public GatheringDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<GatheringShard> GetGatheringInformationAsync(ulong userId, ulong gatheringId)
        {
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is allowed to view gathering
			Try(await targetGathering.IsVisibleTo(user),
				new InvalidGatheringException("User is unable to view gathering."));

			return targetGathering.ToGatheringShard();
		}

		public async Task<List<GatheringShard>> GetGatheringsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUserAsync(userId);
			var nearbyGatherings = await Gatherings.FindGatheringsAsync(latitude, longitude, distance);

			// Remove gatherings from list that the user cannot access
			var filteredGatherings = await RemoveInaccessibleGatheringsAsync(user, nearbyGatherings);

			return filteredGatherings;
		}

		public async Task<List<GatheringShard>> GetPersonalisedGatheringsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUserAsync(userId);
			var nearbyGatherings = await Gatherings.FindGatheringsAsync(latitude, longitude, distance);

			// Remove inaccessible gatherings and gatherings with a large difference between gathering and user interest
			var filteredGatherings = await RemoveUnattractiveGatheringsAsync(user, nearbyGatherings, 1f);

			return filteredGatherings;
		}

		public async Task<GatheringShard> CreateGatheringAsync(ulong userId,
			string gatheringName, string gatheringDescription, DateTimeOffset startTime,
			double latitude, double longitude,
			double radius, bool isDynamic,
			int? groupMinimum, int? groupMaximum,
			MemoryStream heroImage)
		{
			var user = await GetUserAsync(userId);
			// Verify user is already at an gathering
			await ThrowIfUserAtGathering(user);

			// Verify user can host
			Try(user.CanHost,
				new InvalidUserException("User cannot host.\n" +
				$"Account Status: {user.AccountStatus}"));

			// Verify user has position enabled
			Try((await user.LastKnownLocation).Exists,
				new InvalidUserException("User must have location enabled in order to host."));

			// Create gathering
			Gathering gatheringStub = new()
			{
				Name = gatheringName,
				Description = gatheringDescription,
				StartTime = startTime,
				Location = new() { Latitude = latitude, Longitude = longitude },
				GroupMinimum = groupMinimum ?? 0,
				GroupMaximum = groupMaximum ?? 0,
				Radius = new() { Kilometres = Math.Clamp(radius, 0.1, radius) },
				IsDynamic = isDynamic,
			};

			// Validate gathering
			Try(gatheringStub.ValidateAndNormalise(out string issues),
				new InvalidInformationException($"Invalid gathering details provided. Issues: {issues}"));

			// Verify user has no conflict
			var conflict = (await user.UpcomingGatherings).Find(e => IsWithin(e.StartTime - gatheringStub.StartTime, HalfHour));
			if (conflict != null)
			{ throw new InvalidGatheringException($"User has gathering {conflict.Id} conflict."); }

			// Try to create an gathering
			Gathering newGathering = new(await Gatherings.CreateGatheringAsync(user.Id, gatheringStub.Name, gatheringStub.Description,
				gatheringStub.StartTime, gatheringStub.Location.Latitude, gatheringStub.Location.Longitude,
				gatheringStub.GroupMinimum, gatheringStub.GroupMaximum, user.Character.ToCharacter(),
				gatheringStub.Radius.Kilometres, gatheringStub.IsDynamic));

			// Upload hero
			await Terminal.MediaDirector.UploadHeroAsync(newGathering.Id, heroImage);

			// Notify appreciateers of gathering
			_ = user.NotifyAppreciateers($"New Sparrow Gathering", $"{user.Name} just created a new gathering {newGathering.Name}");

			return newGathering.ToGatheringShard();
		}

		public async Task EditGatheringAsync(ulong userId, ulong gatheringId,
			string gatheringDescription = "", bool? isOpen = null,
			DateTimeOffset? startTime = null, double? latitude = null, double? longitude = null,
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is gathering host
			Try(targetGathering.IsModifiableBy(user),
				new InvalidGatheringException("User is unable to edit gathering."));

			// Verify gathering is still active
			Try(targetGathering.IsActive,
				new InvalidGatheringException("Unable to edit gathering, gathering has ended."));

			// Fail if edits may not be done during the gathering
			Fail(HasAlready(targetGathering.StartTime) &&
				(!string.IsNullOrEmpty(gatheringDescription) || IsNotNull(startTime) ||
				AreNotNull(latitude, longitude) ||
				IsNotNull(radius) || IsNotNull(isDynamic)),
				new InvalidGatheringException("Cannot edit certain gathering attributes once it has started."));

			Gathering editedGathering = new(targetGathering.ToGatheringShard())
			{
				Description = gatheringDescription,
				State = IsNull(isOpen) ? targetGathering.State : (isOpen.Value ? GatheringState.Open : GatheringState.Sealed),
				StartTime = startTime ?? targetGathering.StartTime,
				Location = AreNull(latitude, longitude) ? targetGathering.Location : new() { Latitude = latitude.Value, Longitude = longitude.Value },
				Radius = IsNull(radius) ? targetGathering.Radius : new() { Kilometres = Math.Clamp(radius.Value, 0.1, radius.Value) },
				IsDynamic = isDynamic ?? targetGathering.IsDynamic,
				GroupMinimum = groupMinimum ?? targetGathering.GroupMinimum,
				GroupMaximum = groupMaximum ?? targetGathering.GroupMaximum,
			};

			// Validate gathering
			Try(editedGathering.ValidateAndNormalise(out string issues),
				new InvalidInformationException($"Invalid gathering details provided. Issues: {issues}"));

			List<(string Property, object Value)> edits = new();

			// Gather individual edits
			if (!string.IsNullOrEmpty(gatheringDescription))
			{
				edits.Add((nameof(CoreGathering.Description), editedGathering.Description));
			}
			if (IsNotNull(isOpen))
			{
				edits.Add((nameof(CoreGathering.State), editedGathering.State));
			}
			if (IsNotNull(startTime))
			{
				edits.Add((nameof(CoreGathering.StartTime), editedGathering.StartTime));
			}
			if (IsNotNull(latitude) && IsNotNull(longitude))
			{
				edits.Add(("Location", (editedGathering.Location.Latitude, editedGathering.Location.Longitude)));
			}
			if (IsNotNull(radius))
			{
				edits.Add((nameof(CoreGathering.Radius), editedGathering.Radius.Kilometres));
			}
			if (IsNotNull(isDynamic))
			{
				edits.Add((nameof(CoreGathering.IsDynamic), editedGathering.IsDynamic));
			}
			if (IsNotNull(groupMinimum))
			{
				edits.Add((nameof(CoreGathering.GroupMinimum), editedGathering.GroupMinimum));
			}
			if (IsNotNull(groupMaximum))
			{
				edits.Add((nameof(CoreGathering.GroupMaximum), editedGathering.GroupMaximum));
			}

			// Push update
			await Gatherings.UpdateGatheringAsync(targetGathering.Id, edits);

			_ = targetGathering.NotifyActive($"{targetGathering.Name}", "The gathering was edited by the host, check to see the updates.");
		}

		public async Task StartGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);
			_ = targetGathering.Host.LastKnownLocation.Sync();

			// Verify user is host
			Try(targetGathering.IsHostedBy(user),
				new InvalidUserException("User is not the host of this gathering"));

			// Verify gathering can be started
			Try(await targetGathering.IsStartable(),
				new InvalidGatheringException("Gathering cannot be started."));

			// Try to start gathering
			await Gatherings.UpdateGatheringAsync(targetGathering.Id, new() { (nameof(CoreGathering.State), GatheringState.Open) });
			await Gatherings.SetUserStateAsync(user.Id, targetGathering.Id, GatheringBond.Arrived, Time);

			await targetGathering.Started();
		}

		public async Task EndGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify user is able to end the gathering
			Try(gathering.IsModifiableBy(user),
				new InvalidUserException("User does not have permissions to end gathering."));

			// Try to end to gathering
			await Gatherings.EndGatheringAsync(gathering.Id, Time);

			var participants = await gathering.Ended();

			// Update all participants' vectors
			_ = Terminal.AccountDirector.UpdateAllAsync(participants, user => new() { (nameof(CoreUser.Character), user.Character) });
		}

		public async Task DeleteGatheringAsync(ulong userId, ulong gatheringId)
		{
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			// Verify gathering has not yet started
			Fail(gathering.IsOngoing || gathering.IsEnded,
				new InvalidGatheringException("Gathering cannot be deleted once it has started."));

            // Verify user is able to delete the gathering
            Try(gathering.IsModifiableBy(user),
                new InvalidUserException("User does not have permissions to delete gathering."));

			// Try to end to delete gathering
			await Gatherings.DeleteGatheringAsync(gathering.Id);

			// Delete hero
			await Media.DeleteHeroAsync(gathering.Id);

            _ = gathering.NotifyActive($"{gathering.Name}", "Uh oh! The gathering was deleted by the host.");
        }

        public async Task SurveyGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is allowed to view gathering
			Try(await targetGathering.IsVisibleTo(user),
				new InvalidGatheringException($"User is unable to survey gathering.\nAccount Status: {user.AccountStatus}"));

			Fail(targetGathering.EndTime.HasValue,
				new InvalidGatheringException("User is unable to survey gathering, gathering has ended."));

			GatheringBond? userIntention = null;

			try
			{
				userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);
			}
			catch { }

			// Ensure correct state transition
			if (!userIntention.HasValue)
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(userId, gatheringId, GatheringBond.Surveying, Time);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not survey gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task UnsurveyGatheringAsync(ulong userId, ulong gatheringId)
		{
            GatheringBond? userIntention = null;

            try
            {
                userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);
            }
            catch { }

			// Ensure correct state transition
			if (userIntention.HasValue &&
				userIntention.Value.Equals(GatheringBond.Surveying))
			{
				// Try to remove user from gathering
				await Gatherings.RemoveUserAsync(userId, gatheringId);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not unsurvey gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task JoinGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);
			_ = user.LastKnownLocation.Sync();

			// Verify user is allowed to join gathering
			Try(await targetGathering.IsJoinableBy(user),
				new InvalidGatheringException($"User is unable to join gathering.\nAccount Status: {user.AccountStatus}"));

			// Check if user has an active gathering conflict
			if (HasAlready(targetGathering.StartTime))
			{ await ThrowIfUserAtGathering(user); }
			else
			{
				// Check if user has an upcoming conflict
				var conflict = (await user.UpcomingGatherings).Find(e => IsWithin(e.StartTime - targetGathering.StartTime, HalfHour));
				if (conflict != null)
				{ throw new InvalidGatheringException($"User has gathering {conflict.Id} conflict."); }
			}
			// Check if gathering is active and user is already there
			if (HasAlready(targetGathering.StartTime) &&
				await targetGathering.IsInRange(user))
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(user.Id, targetGathering.Id, GatheringBond.Arrived, Time);
			}
			else
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(userId, gatheringId, GatheringBond.Guest, Time);
			}			

			// Notify host if gathering has already started
			if (HasAlready(targetGathering.StartTime))
			{ _ = targetGathering.Host.Notify($"Sparrower Inbound", $"{user.Name} is joining your gathering."); }
		}

		public async Task LeaveGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is the host
			Fail(targetGathering.IsHostedBy(user),
				new InvalidUserException("Host cannot leave the gathering."));

            // Get the user's current status
            var userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);
			
			// Check if user is guest or arrived
			if (userIntention.Equals(GatheringBond.Arrived))
			{
				// Try to remove user from gathering
				await Gatherings.SetUserStateAsync(user.Id, targetGathering.Id, GatheringBond.Left, Time);
			}
			else if (userIntention.Equals(GatheringBond.Guest))
			{
				// Try to remove user from gathering
				await Gatherings.RemoveUserAsync(user.Id, targetGathering.Id);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not leave gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task<GuestListShard>
			GetGuestListAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			GuestListShard guestList = new(0, 0, new());

			// Check if user is host
			if (targetGathering.IsModifiableBy(user))
			{
				// Retrieve user's companions that are surveying
				var companions = await targetGathering.GetCompanionsOf(user);

				guestList.Guests.AddRange(SelectAsSilhouette(companions,
					companion => companion.State.Equals(GatheringBond.Surveying)));

				// Add visible users
				guestList.Guests.AddRange(SelectAsSilhouette(await targetGathering.AllUsers,
					user => !user.State.Equals(GatheringBond.Surveying)));

				guestList = guestList with
				{
					GuestCount = targetGathering.IsOngoing ? (await targetGathering.Arrived).Count : (await targetGathering.Left).Count,
					Surveyers = (await targetGathering.Surveying).Count
				};
			}
			// Check if user is a guest
			else if (await targetGathering.WasAttendedBy(user))
			{
				// Retrieve user's companions surveying or attending
				var companions = await targetGathering.GetCompanionsOf(user);

				guestList.Guests.AddRange(SelectAsSilhouette(companions,
					companion => companion.State.Equals(GatheringBond.Surveying) || companion.State.Equals(GatheringBond.Guest)));

				// Add visible users
				guestList.Guests.AddRange(SelectAsSilhouette(await targetGathering.AllUsers,
					user => user.State.Equals(GatheringBond.Arrived) || user.State.Equals(GatheringBond.Left)));

				guestList = guestList with
				{
					GuestCount = targetGathering.IsOngoing ? (await targetGathering.Arrived).Count : (await targetGathering.Left).Count,
					Surveyers = guestList.Guests.Where(guest => guest.Bond.Equals(GatheringBond.Surveying)).Count()
				};
            }
			// Check if user can view gathering
			else if (await targetGathering.IsVisibleTo(user))
			{
				// Retrieve user's companions that will be, are, or were attending
				var companions = await targetGathering.GetCompanionsOf(user);
				guestList = new(0, 0, SelectAsSilhouette(companions, _ => true));

				// Add visible information
				guestList = guestList with
				{
					GuestCount = targetGathering.IsOngoing ? (await targetGathering.Arrived).Count : (await targetGathering.Left).Count,
					Surveyers = SelectAsSilhouette(companions, companion => companion.State.Equals(GatheringBond.Surveying)).Count
				};
			}
			// User cannot recieve information about gathering
			else
			{ throw new InvalidUserException("User cannot view gathering."); }

			return guestList;
		}

		public async Task<List<UserSilhouette>> GetPotentialInviteesAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			List<User> potentialUsers = new();

			// Add all companions that can join gathering
			foreach (var companion in await user.Companions)
			{
				if (await gathering.IsJoinableBy(companion))
				{ potentialUsers.Add(companion); }
			}

			return potentialUsers
				.ConvertAll(u => u.ToUserSilhouette());
		}

		public async Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong gatheringId)
		{
			var inviter = await GetUserAsync(inviterId);
			var invitee = await GetUserAsync(inviteeId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify inviter has relationship with gathering
			Try(await gathering.HasUserRelationship(inviter),
				new InvalidGatheringException("User must be surveying, a guest, or arrived at gathering to invite."));

			// Verify that the invitee can join the gathering
			Try(await gathering.IsJoinableBy(invitee),
				new InvalidUserException("Invited cannot join gathering."));

			// Verify that inviter is companions with the invitee
			Try(await inviter.IsCompanionsWith(invitee),
				new InvalidUserException("Cannot invite non-companions."));

			_ = invitee.PostNote(inviter, $"has invited you to {gathering.Name}", $"{gathering.Id}");
			_ = invitee.Notify("Sparrow", "You were invited to ");
		}

		public async Task KickUserAsync(ulong hostId, ulong targetId, ulong gatheringId)
		{
			var host = await GetUserAsync(hostId);
			var targetUser = await GetUserAsync(targetId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify kicking user is the host
			Try(gathering.IsHostedBy(host),
				new InvalidUserException("User cannot kick guests."));

			// Verify gathering is active
			Try(gathering.IsActive,
				new InvalidGatheringException("Cannot kick users after gathering has been archived."));

			// Verify host is not kicking themself
			Fail(host.Equals(targetUser),
				new InvalidUserException("Host cannot kick themself."));

			// Kick target user from gathering
			await Gatherings.SetUserStateAsync(targetUser.Id, gathering.Id, GatheringBond.Kicked, Time);

			// Hide target user's snapshots from gathering
			foreach (SnapshotShard snapshot in await gathering.Snapshots)
			{
				if (targetUser.Taken(snapshot))
				{ _ = Snapshots.HideSnapshotAsync(snapshot.Id); }
			}
		}

		#endregion

		#region Favours

		internal async Task<Gathering> RequestCurrentGatheringForUserAsync(User user)
		{
			var currentGathering = await Gatherings.FindCurrentGatheringForUserAsync(user.Id);

			return currentGathering != null ? new(currentGathering) : Gathering.None;
		}

		internal async Task<List<Gathering>> RequestPastGatheringsForUserAsync(User user)
		{
			return (await Gatherings.FindPastGatheringsForUserAsync(user.Id))
				.ConvertAll(gathering => new Gathering(gathering));
		}

		internal async Task<List<Gathering>> RequestUpcomingGatheringsForUserAsync(User user)
		{
			return (await Gatherings.FindUpcomingGatheringsForUserAsync(user.Id))
				.ConvertAll(gathering => new Gathering(gathering));
		}

		internal async Task<List<Gathering>> RequestSurveyingGatheringsForUserAsync(User user)
		{
			return (await Gatherings.FindSurveyingGatheringsForUserAsync(user.Id))
				.ConvertAll(gathering => new Gathering(gathering));
		}
		
		internal async Task<List<(User User, GatheringBond State)>> RequestAllUsersFromGatheringAsync(Gathering gathering)
		{
			return (await Gatherings.GetAllUsersAsync(gathering.Id))
				.ConvertAll(userDetails => (new User(userDetails.User), userDetails.State));
		}

		internal async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, User User)>>
			RequestGuestHistoryAsync(Gathering gathering)
		{
			return (await Gatherings.GetGuestHistoryAsync(gathering.Id))
				.ConvertAll(userDetails => (userDetails.Joined, userDetails.Left, new User(userDetails.User)));
		}

		internal async Task<List<GatheringShard>>
			RemoveInaccessibleGatheringsAsync(User user, List<CoreGathering> gatherings)
		{
			List<GatheringShard> accessibleGatherings = new();

			foreach (CoreGathering coreGathering in gatherings)
			{
				Gathering gathering = new(coreGathering);

				if (await user.CanJoin(gathering))
				{ accessibleGatherings.Add(gathering.ToGatheringShard(user)); }
			}

			return accessibleGatherings;
		}

		internal async Task<AgendaShard>
			RemoveInaccessibleGatheringBondsAsync(User user, AgendaShard agenda)
		{
			AgendaShard accessibleGatherings = new(new());

			foreach ((GatheringShard shard, GatheringBond bond) in agenda.Agenda)
			{
				Gathering gathering = new(shard);

				if (await user.CanJoin(gathering))
				{ accessibleGatherings.Agenda.Add((shard, bond)); }
			}

			return accessibleGatherings;
		}

		internal async Task<List<GatheringShard>>
			RemoveUnattractiveGatheringsAsync(User user, List<CoreGathering> gatherings, float maximumAngle)
        {
            List<GatheringShard> accessibleGatherings = new();

            foreach (CoreGathering coreGathering in gatherings)
			{
				Gathering gathering = new(coreGathering);

				gathering.RelativeAngle = CharacterVector.AngleBetweenAffected(user.Character, gathering.Character);

                if (await user.CanJoin(gathering))
				{ accessibleGatherings.Add(gathering.ToGatheringShard()); continue; }

				if (gathering.RelativeAngle < maximumAngle)
				{ accessibleGatherings.Add(gathering.ToGatheringShard()); }
            }

            return accessibleGatherings;
		}

		#endregion

		#region Tools

		private async Task ThrowIfUserAtGathering(User user)
		{
			Fail(await user.IsAtGathering(),
				new InvalidUserException($"{user.Name} is currently attending the gathering {(await user.CurrentGathering).Name}."));
		}

		private List<(UserSilhouette User, GatheringBond State)>
			SelectAsSilhouette(List<(User User, GatheringBond State)> users, Func<(User User, GatheringBond State), bool> predicate)
		{
			return users.Where(predicate).ToList().ConvertAll(userDetails => (userDetails.User.ToUserSilhouette(), userDetails.State));
		}

		#endregion
	}
}
