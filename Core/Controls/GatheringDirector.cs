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
			Verify(await targetGathering.IsVisibleTo(user),
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

			// Ensure user's own and current gatherings show
			/*
			var upcomingGatherings = await Gatherings.FindUpcomingGatheringsForUserAsync(user.Id);
			upcomingGatherings.Add(await Gatherings.FindCurrentGatheringForUserAsync(user.Id));

			filteredGatherings = EnsureContains(filteredGatherings, upcomingGatherings);
			*/

			return filteredGatherings;
		}

		public async Task<List<GatheringShard>> GetPersonalisedGatheringsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUserAsync(userId);
			var nearbyGatherings = await Gatherings.FindGatheringsAsync(latitude, longitude, distance);

			// Remove inaccessible gatherings and gatherings with a large difference between gathering and user interest
			var filteredGatherings = await RemoveUnattractiveGatheringsAsync(user, nearbyGatherings, 1f);

            // Ensure user's own and current gatherings show
			/*
            var upcomingGatherings = await Gatherings.FindUpcomingGatheringsForUserAsync(user.Id);
            upcomingGatherings.Add(await Gatherings.FindCurrentGatheringForUserAsync(user.Id));

            filteredGatherings = EnsureContains(filteredGatherings, upcomingGatherings);
			*/

            return filteredGatherings;
		}

		public async Task<GatheringShard> CreateGatheringAsync(ulong userId,
			string gatheringName, string gatheringDescription, DateTimeOffset startTime,
			double latitude, double longitude, string friendlyLocation,
			double radius, bool isDynamic,
			int? groupMinimum, int? groupMaximum,
			MemoryStream heroImage)
		{
			var user = await GetUserAsync(userId);

			// Verify user can host
			Verify(user.CanHost,
				new InvalidUserException("User cannot host.\n" +
				$"Account Status: {user.AccountStatus}"));

			// Verify user has position enabled
			Verify((await user.LastKnownLocation).Exists,
				new InvalidUserException("User must have location enabled in order to host."));

			// Create gathering
			Gathering gatheringStub = new()
			{
				Name = gatheringName,
				Description = gatheringDescription,
				StartTime = startTime,
				Location = new() { Latitude = latitude, Longitude = longitude },
				FriendlyLocation = friendlyLocation,
				GroupMinimum = groupMinimum ?? 0,
				GroupMaximum = groupMaximum ?? 0,
				Radius = new() { Kilometres = Math.Clamp(radius, 0.1, radius) },
				IsDynamic = isDynamic,
			};

			// Validate gathering
			Verify(gatheringStub.ValidateAndNormalise(out string issues),
				new InvalidInformationException($"Invalid gathering details provided. Issues: {issues}"));

			// Verify user has no conflict
			var conflict = (await user.UpcomingGatherings).Find(e => IsWithin(e.StartTime - gatheringStub.StartTime, HalfHour));
			if (conflict != null)
			{ throw new InvalidGatheringException($"User has gathering {conflict.Id} conflict."); }

			// Try to create a gathering
			Gathering newGathering = new(await Gatherings.CreateGatheringAsync(user.Id, gatheringStub.Name, gatheringStub.Description,
				gatheringStub.StartTime,
				gatheringStub.Location.Latitude, gatheringStub.Location.Longitude, gatheringStub.FriendlyLocation,
				gatheringStub.GroupMinimum, gatheringStub.GroupMaximum, user.Character.ToCharacter(),
				gatheringStub.Radius.Kilometres, gatheringStub.IsDynamic));

			try
			{
				// Upload hero
				await Terminal.MediaDirector.UploadHeroAsync(newGathering.Id, heroImage);
			}
			catch
			{
				// If failed, remove gathering
				await Gatherings.DeleteGatheringAsync(newGathering.Id);
				throw new UnexpectedFailureException("Failed to upload hero image.");
            }

            // If now
			if (HasAlready(newGathering.StartTime))
			{
				// Ensure user removed from current gathering
				if (await user.IsAtGathering())
				{
					await LeaveGatheringAsync(user.Id, (await user.CurrentGathering).Id);
				}

				// Try to start gathering
				try
				{
					await StartGatheringAsync(user.Id, newGathering.Id);
					await Gatherings.UpdateGatheringAsync(newGathering.Id, new() { (nameof(CoreGathering.StartTime), Time) });
					newGathering = await GetGatheringAsync(newGathering.Id);
				}
				catch { }
			}

            // Notify appreciateers of gathering
            _ = user.NotifyAppreciateers($"New Sparrow Gathering", $"{user.Name} just created a new gathering {newGathering.Name}");

			return newGathering.ToGatheringShard();
		}

		public async Task EditGatheringAsync(ulong userId, ulong gatheringId,
			string gatheringName = "", string gatheringDescription = "", bool? isOpen = null,
			DateTimeOffset? startTime = null,
			double? latitude = null, double? longitude = null, string friendlyLocation = "",
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null,
			MemoryStream heroImage = null)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is gathering host
			Verify(targetGathering.IsModifiableBy(user),
				new InvalidGatheringException("User is unable to edit gathering."));

			// Ensure gathering is editable
			FailIf(targetGathering.IsTerminated,
				new InvalidGatheringException("Unable to edit gathering, gathering has ended."));

			// Fail if edits may not be done during the gathering
			FailIf(targetGathering.IsOngoing &&
				(!string.IsNullOrEmpty(gatheringName) ||
				!string.IsNullOrEmpty(gatheringDescription) ||
				IsNotNull(startTime) ||
				AreNotNull(latitude, longitude) ||
                !string.IsNullOrEmpty(friendlyLocation) ||
                IsNotNull(radius) || IsNotNull(isDynamic)),
				new InvalidGatheringException("Cannot edit certain gathering attributes once it has started."));

			Gathering editedGathering = new(targetGathering.ToGatheringShard())
			{
                Name = string.IsNullOrEmpty(gatheringName) ? targetGathering.Name : gatheringName,
                Description = string.IsNullOrEmpty(gatheringDescription) ? targetGathering.Description : gatheringDescription,
				State = IsNull(isOpen) ? targetGathering.State : (isOpen.Value ? GatheringState.Open : GatheringState.Sealed),
				StartTime = startTime ?? targetGathering.StartTime,
				Location = AreNull(latitude, longitude) ? targetGathering.Location : new() { Latitude = latitude.Value, Longitude = longitude.Value },
				FriendlyLocation = string.IsNullOrEmpty(friendlyLocation) ? targetGathering.FriendlyLocation : friendlyLocation,
				Radius = IsNull(radius) ? targetGathering.Radius : new() { Kilometres = Math.Clamp(radius.Value, 0.1, radius.Value) },
				IsDynamic = isDynamic ?? targetGathering.IsDynamic,
				GroupMinimum = groupMinimum ?? targetGathering.GroupMinimum,
				GroupMaximum = groupMaximum ?? targetGathering.GroupMaximum,
			};

			// Validate gathering
			Verify(editedGathering.ValidateAndNormalise(out string issues),
				new InvalidInformationException($"Invalid gathering details provided. Issues: {issues}"));

			List<(string Property, object Value)> edits = new();

			// Gather individual edits
			if (!string.IsNullOrEmpty(gatheringName))
			{
				edits.Add((nameof(CoreGathering.Name), editedGathering.Name));
			}
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
			if (!string.IsNullOrEmpty(friendlyLocation))
			{
				edits.Add((nameof(CoreGathering.FriendlyLocation), editedGathering.FriendlyLocation));
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

			// Update hero image if provided
			if (heroImage != null && heroImage.Length > 0)
			{
				await Terminal.MediaDirector.UploadHeroAsync(targetGathering.Id, heroImage);
            }

			_ = targetGathering.NotifyActive($"{targetGathering.Name}", "The gathering was edited by the host, check to see the updates!");
		}

		public async Task StartGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);
			_ = gathering.Host.LastKnownLocation.Sync();

			// Verify user is host
			Verify(gathering.IsHostedBy(user),
				new InvalidUserException("User is not the host of this gathering"));

			// Verify gathering can be started
			Verify(await gathering.IsStartable(),
				new InvalidGatheringException("Gathering cannot be started."));

			// Try to start gathering
			await Gatherings.UpdateGatheringAsync(gathering.Id, new() { (nameof(CoreGathering.State), GatheringState.Open), (nameof(CoreGathering.StartTime), Time) });
			await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Arrived, Time);

			await gathering.Started();
		}

		public async Task TerminateGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify user is able to end the gathering
			Verify(gathering.IsModifiableBy(user),
				new InvalidUserException("User does not have permissions to end gathering."));

			// Verify gathering is able to be terminated
            Verify(gathering.IsTerminable(),
                new InvalidGatheringException("Gathering cannot be terminated."));

            // Try to end gathering
            await Gatherings.TerminateGatheringAsync(gathering.Id, Time);

			var participants = await gathering.Ended();

			// Update all participants' vectors
			_ = Terminal.AccountDirector.UpdateAllAsync(participants, user => new() { (nameof(CoreUser.Character), user.Character) });

			if (gathering.Duration > TimeSpan.FromMinutes(30))
			{
				// Notify no-shows
				var absentUsers = (await gathering.Guests).Except(await gathering.Arrived).Except(await gathering.Left);

				foreach (var absent in absentUsers)
				{
					await absent.PostTelegram(User.Hollow, TelegramMessage.GatheringMissedAttendee, $"{gathering.Name}");
				}
			}
		}

		public async Task DeleteGatheringAsync(ulong userId, ulong gatheringId)
		{
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			// Verify gathering has not yet started
			Verify(gathering.IsDeletable(),
				new InvalidGatheringException("Gathering cannot be deleted once it has started."));

            // Verify user is able to delete the gathering
            Verify(gathering.IsModifiableBy(user),
                new InvalidUserException("User does not have permissions to delete gathering."));

			// Try to delete gathering
			await Gatherings.DeleteGatheringAsync(gathering.Id);

			// Delete hero
			await Media.DeleteHeroAsync(gathering.Id);

            _ = gathering.NotifyActive($"{gathering.Name}", "Uh oh! The gathering was deleted by the host.");
        }

        public async Task WatchGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify user is allowed to view gathering
			Verify(await gathering.IsVisibleTo(user),
				new InvalidGatheringException($"User is unable to watch gathering.\nAccount Status: {user.AccountStatus}"));

			FailIf(gathering.EndTime.HasValue,
				new InvalidGatheringException("User is unable to watch gathering, gathering has ended."));

			GatheringBond? userIntention = null;

			try
			{
				userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);
			}
			catch { }

			// Check that user was not kicked
			FailIf(userIntention.HasValue &&
				userIntention.Value.Equals(GatheringBond.Kicked),
				new InvalidUserException($"Could not watch gathering, user was kicked."));

            // Ensure correct state transition
            if (!userIntention.HasValue)
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Watching, Time);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Cannot watch gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task UnwatchGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);

            GatheringBond? userIntention = null;

            try
            {
                userIntention = await Gatherings.GetUserStateAsync(user.Id, gatheringId);
            }
            catch { }

            // Check that user was not kicked
            FailIf(userIntention.HasValue &&
                userIntention.Value.Equals(GatheringBond.Kicked),
                new InvalidUserException($"Cannot unwatch gathering, user was kicked."));

            // Ensure correct state transition
            if (userIntention.HasValue &&
				userIntention.Value.Equals(GatheringBond.Watching))
			{
				// Try to remove user from gathering
				await Gatherings.DeleteUserStateAsync(user.Id, gatheringId);
            }
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not unwatch gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task JoinGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);
			_ = user.LastKnownLocation.Sync();

			// Verify user is allowed to join gathering
			Verify(await gathering.IsJoinableBy(user),
				new InvalidGatheringException($"User is unable to join gathering.\nAccount Status: {user.AccountStatus}"));

            GatheringBond? previousUserState = null;

            try
            {
                previousUserState = await Gatherings.GetUserStateAsync(userId, gatheringId);
            }
            catch { }

            // Check that user was not kicked
            FailIf(previousUserState.HasValue &&
                previousUserState.Value.Equals(GatheringBond.Kicked),
                new InvalidUserException($"Could not join gathering, user was kicked."));

            // Check if user is already guest or arrived
            if (previousUserState.HasValue &&
				(previousUserState.Value.Equals(GatheringBond.Guest) ||
                previousUserState.Value.Equals(GatheringBond.Arrived)))
			{
                throw new InvalidUserException($"User already joined gathering.");
            }
            // Check if user has an active gathering conflict
            if (HasAlready(gathering.StartTime))
			{ await ThrowIfUserAtGathering(user); }
			else
			{
				// Check if user has an upcoming conflict
				var conflict = (await user.UpcomingGatherings).Find(e => IsWithin(e.StartTime - gathering.StartTime, HalfHour));
				if (conflict != null)
				{ throw new InvalidGatheringException($"User has gathering {conflict.Id} conflict."); }
			}
			// Check if gathering is active and user is already there
			if (HasAlready(gathering.StartTime) &&
				await gathering.IsInRange(user))
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Arrived, Time);
			}
			else
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(user.Id, gatheringId, GatheringBond.Guest, Time);
            }

			// Notify host if gathering has already started
			if (HasAlready(gathering.StartTime))
			{ _ = gathering.Host.Notify($"Sparrower Inbound", $"{user.Name} is joining your gathering."); }
		}

		public async Task CheckInToGatheringAsync(ulong userId, double latitude, double longitude)
        {
            var user = await GetUserAsync(userId);
            var nextGathering = await user.NextGathering();

            user.LastKnownLocation.Set(new() { Latitude = latitude, Longitude = longitude });

            // Position update
            _ = Accounts.UpdateRecentLocationAsync(user.Id,
                (await user.LastKnownLocation).Latitude,
                (await user.LastKnownLocation).Longitude,
                (await user.LastKnownRadius).Metres);

			FailIf(await user.IsAtGathering(),
				new InvalidActionException("User is currently attending another gathering."));
			FailIf(nextGathering.Equals(Gathering.None) || !nextGathering.IsOngoing,
                new InvalidActionException("User does not have an available gathering to check-in to."));
            // FailIf(!await nextGathering.IsInRange(user),
            //     new InvalidActionException("User is not in range of the gathering."));
            
			await Gatherings.SetUserStateAsync(user.Id, nextGathering.Id, GatheringBond.Arrived, Time);
        }

		public async Task LeaveGatheringAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is the host
			FailIf(targetGathering.IsHostedBy(user),
				new InvalidUserException("Host cannot leave the gathering."));

            // Get the user's current status
            var userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);

            // Check that user was not kicked
            FailIf(userIntention.HasValue &&
                userIntention.Value.Equals(GatheringBond.Kicked),
                new InvalidUserException($"Could not leave gathering, user was kicked."));

            // Check if user is guest or arrived
            if (userIntention.Equals(GatheringBond.Arrived))
			{
				// Try to remove user from gathering
				await Gatherings.SetUserStateAsync(user.Id, targetGathering.Id, GatheringBond.Left, Time);
			}
			else if (userIntention.Equals(GatheringBond.Guest))
			{
				// Check if user previously left gathering
				if (await targetGathering.WasAttendedBy(user))
				{
					// TODO This should not create false data.
					await Gatherings.SetUserStateAsync(user.Id, targetGathering.Id, GatheringBond.Left, Time);
				}
				else
				{
					// Try to remove user from gathering
					await Gatherings.DeleteUserStateAsync(user.Id, targetGathering.Id);
				}
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not leave gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task<List<GuestListBondPair>>
			GetGuestListAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Gather
			var allGuests = SelectAsShard(await gathering.AllUsers,
				user => user.State != GatheringBond.Watching);

			// Sort
			allGuests.Sort((bond1, bond2) =>
			{
				return bond1.User.Name.CompareTo(bond2.User.Name);
            });

			// Hide

			// Check if user is host or attendee
			if (gathering.IsModifiableBy(user) || await gathering.WasAttendedBy(user))
            {
				for (int i = 0; i < allGuests.Count; i++)
				{
					User guest = new(allGuests[i].User);
					GatheringBond bond = allGuests[i].Bond;

					// Might have to hide incoming guest
					if (bond == GatheringBond.Guest)
					{
						bool isCompanion = await user.IsCompanionsWith(guest);
						bool isHost = gathering.IsModifiableBy(guest);
						bool isSelf = user.Equals(guest);

						// Check if incoming guest is not a companion, host, or self
						if (!(isCompanion || isHost || isSelf))
						{
							allGuests[i] = AsHiddenBondPair(bond);
						}
					}
					// Else, guest is arrived or left (visible)
				}
			}
			// Check if user can view gathering
			else if (await gathering.IsVisibleTo(user))
			{
                for (int i = 0; i < allGuests.Count; i++)
                {
                    User guest = new(allGuests[i].User);

                    bool isCompanion = await user.IsCompanionsWith(guest);
                    bool isHost = gathering.IsModifiableBy(guest);
                    bool isSelf = user.Equals(guest);

					// Not attending so can only see select people
                    // Check if guest is not a companion, host, or self
                    if (!(isCompanion || isHost || isSelf))
                    {
                        allGuests[i] = AsHiddenBondPair(allGuests[i].Bond);
                    }
                }
            }
			// User cannot recieve information about gathering
			else
			{ throw new InvalidUserException("User cannot view gathering."); }

            return allGuests;
		}

		public async Task<List<UserShard>> GetPotentialInviteesAsync(ulong userId, ulong gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			List<User> potentialUsers = new();

			// Check companions
			foreach (var companion in await user.Companions)
			{
				// Verify they can join and are not already on the guest list
				if (await gathering.IsJoinableBy(companion) &&
					!await gathering.HasOnGuestList(companion))
				{ potentialUsers.Add(companion); }
			}

			return potentialUsers
				.ConvertAll(u => u.ToUserShard());
		}

		public async Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong gatheringId)
		{
			var inviter = await GetUserAsync(inviterId);
			var invitee = await GetUserAsync(inviteeId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify inviter has relationship with gathering
			Verify(await gathering.HasUserRelationship(inviter),
				new InvalidGatheringException("User must be surveying, a guest, or arrived at gathering to invite."));

			// Verify that the invitee can join the gathering
			Verify(await gathering.IsJoinableBy(invitee),
				new InvalidUserException("Invited cannot join gathering."));

			// Verify that inviter is companions with the invitee
			Verify(await inviter.IsCompanionsWith(invitee),
				new InvalidUserException("Cannot invite non-companions."));

			_ = invitee.PostTelegram(inviter, TelegramMessage.GatheringInvitation, $"{gathering.Id}");
			_ = invitee.Notify("Canary", $"You were invited to {gathering.Name}");
		}

		public async Task KickUserAsync(ulong hostId, ulong targetId, ulong gatheringId)
		{
			var host = await GetUserAsync(hostId);
			var targetUser = await GetUserAsync(targetId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify kicking user is the host
			Verify(gathering.IsHostedBy(host),
				new InvalidUserException("User cannot kick guests."));

			// Verify gathering is active
			Verify(gathering.IsActive,
				new InvalidGatheringException("Cannot kick users after gathering has been archived."));

			// Verify host is not kicking themself
			FailIf(host.Equals(targetUser),
				new InvalidUserException("Host cannot kick themself."));

			// Kick target user from gathering
			await Gatherings.SetUserStateAsync(targetUser.Id, gathering.Id, GatheringBond.Kicked, Time);

			// Remove target user's snapshots from gathering
			foreach (SnapshotShard snapshot in await gathering.Snapshots)
			{
				if (targetUser.Taken(snapshot))
				{ _ = Snapshots.DeleteSnapshotAsync(snapshot.Id); }
			}
		}

		public async Task<bool> AuthorisedToStart(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			return gathering.IsHostedBy(user) && await gathering.IsStartable();
        }

		public async Task<bool> AuthorisedToJoin(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			return await user.CanJoin(gathering);
        }

		public async Task<bool> AuthorisedToUpload(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			return gathering.IsActive && await gathering.WasAttendedBy(user);
        }

		#endregion

		#region Favours

		internal async Task<Gathering> RequestCurrentGatheringForUserAsync(User user)
		{
			CoreGathering currentGathering;

			try
			{
				currentGathering = await Gatherings.FindCurrentGatheringForUserAsync(user.Id);
			}
			catch { return Gathering.None; }

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
			RemoveUnviewableAgendaCardsAsync(User user, AgendaShard agenda)
		{
			AgendaShard viewableGatherings = new(new());

			foreach (var card in agenda.Cards)
			{
				Gathering gathering = await GetGatheringAsync(card.GatheringId);

				if (await user.CanView(gathering))
				{ viewableGatherings.Cards.Add(card); }
			}

			return viewableGatherings;
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

		internal List<GatheringShard>
			EnsureContains(List<GatheringShard> list, List<CoreGathering> ensured)
		{
			foreach (CoreGathering gathering in ensured)
            {
                // Has match
                var pair = list.Find(g => g.Id.Equals(gathering.Id));

				// Add if not default
				if (!pair.Equals(default))
				{
					Gathering gath = new(gathering);
					list.Add(gath.ToGatheringShard());
				}
			}

			return list;
		}

		#endregion

		#region Tools

		private async Task ThrowIfUserAtGathering(User user)
		{
			FailIf(await user.IsAtGathering(),
				new InvalidUserException($"{user.Name} is currently attending the gathering {(await user.CurrentGathering).Name}."));
		}

		private List<GuestListBondPair>
			SelectAsShard(List<(User User, GatheringBond State)> users, Func<(User User, GatheringBond State), bool> predicate)
		{
			return users.Where(predicate).ToList().ConvertAll(userDetails => new GuestListBondPair(userDetails.User.ToUserShard(), userDetails.State));
		}

		private GuestListBondPair AsHiddenBondPair(GatheringBond bond)
		{
			return new(new(0, "hidden"), bond);
		}

		#endregion
	}
}
