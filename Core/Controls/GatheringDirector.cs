using Core.Boundaries;
using Core.Entities;
using Core.Notifications;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

		public async Task<GatheringShard> GetGatheringInformationAsync(long userId, long gatheringId)
        {
			var user = await GetUserAsync(userId);
			var targetGathering = await GetGatheringAsync(gatheringId);

			// Verify user is allowed to view gathering
			Verify(await targetGathering.IsVisibleTo(user),
				new UserErrorException(GatheringErrorCode.CANNOT_VIEW));

			return await targetGathering.ToGatheringShard();
		}

		public async Task<List<GatheringShard>> GetGatheringsInAreaAsync(long userId,
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

		public async Task<List<GatheringShard>> GetPersonalisedGatheringsInAreaAsync(long userId,
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

		public async Task<GatheringShard> CreateGatheringAsync(long userId,
			string gatheringName, string gatheringDescription, DateTimeOffset startTime,
			double latitude, double longitude, string friendlyLocation,
			double radius, bool isDynamic, int degreeOfPrivacy,
			int? groupMinimum, int? groupMaximum,
			MemoryStream heroImage)
		{
			var user = await GetUserAsync(userId);

			// Verify user can host
			Verify(user.CanHost,
				new UserErrorException(GatheringErrorCode.CANNOT_HOST, new { user.AccountStatus }));

			// Verify user has position enabled
			Verify((await user.LastKnownLocation).Exists,
				new UserErrorException(GatheringErrorCode.LOCATION_DISABLED));

			// Create gathering
			Gathering gatheringStub = new()
			{
				Title = gatheringName,
				Description = gatheringDescription,
				StartTime = startTime,
				Location = new() { Latitude = latitude, Longitude = longitude },
				FriendlyLocation = friendlyLocation,
				GroupMinimum = groupMinimum ?? 0,
				GroupMaximum = groupMaximum ?? 0,
				Radius = new() { Kilometres = Math.Clamp(radius, 0.1, radius) },
				IsDynamic = isDynamic,
				DegreeOfPrivacy = degreeOfPrivacy,
			};

			// Validate gathering
			Verify(gatheringStub.ValidateAndNormalise(out string issues),
				new UserErrorException(GatheringErrorCode.INVALID_DETAILS, new { issues }));

			// Verify user has no conflict
			var conflict = (await user.UpcomingGatherings).Find(e => IsWithin(e.StartTime - gatheringStub.StartTime, HalfHour));
			if (conflict != null)
			{ throw new UserErrorException(GatheringErrorCode.CONFLICT, new { conflict.Id }); }

			// Try to create a gathering
			Gathering newGathering = new(await Gatherings.CreateGatheringAsync(user.Id,
				gatheringStub.Title, gatheringStub.Description, gatheringStub.StartTime,
				gatheringStub.Location.Latitude, gatheringStub.Location.Longitude, gatheringStub.FriendlyLocation,
				gatheringStub.GroupMinimum, gatheringStub.GroupMaximum, user.Character.ToCharacter(),
				gatheringStub.Radius.Kilometres, gatheringStub.IsDynamic, gatheringStub.DegreeOfPrivacy,
				Time));

			try
			{
				// Upload hero
				await Terminal.MediaDirector.UploadHeroAsync(newGathering.Id, heroImage);
			}
			catch (Exception ex)
			{
				// If failed, remove gathering
				await Gatherings.HardDeleteAsync(newGathering.Id);
				throw new UnexpectedFailureException($"Failed to upload hero image for gathering by {user.Id}.", ex, HollowErrorCode.UPLOAD_FAILED);
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
			else
			{
				// Schedule notifications
				await ScheduleNotifications(newGathering);
			}

            // Notify companions of gathering
            _ = user.NotifyCompanions(CanaryNotification.CompanionGatheringCreated(user.ToUserShard(), await newGathering.ToGatheringShard()));
			
			return await newGathering.ToGatheringShard();
		}

		public async Task EditGatheringAsync(long userId, long gatheringId,
			string gatheringName = "", string gatheringDescription = "",
			DateTimeOffset? startTime = null,
			double? latitude = null, double? longitude = null, string friendlyLocation = "",
			double? radius = null, bool? isDynamic = null, int? degreeOfPrivacy = null,
			int? groupMinimum = null, int? groupMaximum = null,
			MemoryStream heroImage = null)
		{
			var user = await GetUserAsync(userId);
			var originalGathering = await GetGatheringAsync(gatheringId);

			// Verify user is gathering host
			Verify(originalGathering.IsModifiableBy(user),
				new UserErrorException(GatheringErrorCode.CANNOT_EDIT_PERMISSION));

			// Ensure gathering is editable
			FailIf(originalGathering.IsTerminated,
				new UserErrorException(GatheringErrorCode.CANNOT_EDIT_ENDED));

			// Fail if edits may not be done during the gathering
			FailIf(originalGathering.IsOngoing &&
				(!string.IsNullOrEmpty(gatheringName) ||
				!string.IsNullOrEmpty(gatheringDescription) ||
				IsNotNull(startTime) ||
				AreNotNull(latitude, longitude) ||
                !string.IsNullOrEmpty(friendlyLocation) ||
                IsNotNull(radius) || IsNotNull(isDynamic)),
				new UserErrorException(GatheringErrorCode.CANNOT_EDIT_STARTED));

			Gathering editedGathering = new(originalGathering.ToCoreGathering())
			{
                Title = string.IsNullOrEmpty(gatheringName) ? originalGathering.Title : gatheringName,
                Description = string.IsNullOrEmpty(gatheringDescription) ? originalGathering.Description : gatheringDescription,
				StartTime = startTime ?? originalGathering.StartTime,
				Location = AreNull(latitude, longitude) ? originalGathering.Location : new() { Latitude = latitude.Value, Longitude = longitude.Value },
				FriendlyLocation = string.IsNullOrEmpty(friendlyLocation) ? originalGathering.FriendlyLocation : friendlyLocation,
				Radius = IsNull(radius) ? originalGathering.Radius : new() { Kilometres = Math.Clamp(radius.Value, 0.1, radius.Value) },
				IsDynamic = isDynamic ?? originalGathering.IsDynamic,
				DegreeOfPrivacy = degreeOfPrivacy ?? originalGathering.DegreeOfPrivacy,
				GroupMinimum = groupMinimum ?? originalGathering.GroupMinimum,
				GroupMaximum = groupMaximum ?? originalGathering.GroupMaximum,
			};

			// Validate gathering
			Verify(editedGathering.ValidateAndNormalise(out string issues),
				new UserErrorException(GatheringErrorCode.INVALID_DETAILS, new { issues }));

			List<(string Property, object Value)> edits = new();

			// Gather individual edits
			if (!string.IsNullOrEmpty(gatheringName))
			{
				edits.Add((nameof(CoreGathering.Title), editedGathering.Title));
			}
			if (!string.IsNullOrEmpty(gatheringDescription))
			{
				edits.Add((nameof(CoreGathering.Description), editedGathering.Description));
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
			if (IsNotNull(degreeOfPrivacy))
			{
				edits.Add((nameof(CoreGathering.DegreeOfPrivacy), editedGathering.DegreeOfPrivacy));
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
			await Gatherings.UpdateGatheringAsync(originalGathering.Id, edits);

			// Update hero image if provided
			if (heroImage != null && heroImage.Length > 0)
			{
				await Terminal.MediaDirector.UploadHeroAsync(originalGathering.Id, heroImage);
            }

			_ = originalGathering.NotifyActive(CanaryNotification.GatheringEdited(await originalGathering.ToGatheringShard()), notifyHost: false);

			// Reschedule notifications if required
			if (IsNotNull(startTime))
			{
				_ = RescheduleSchedule(editedGathering);
            }
        }

		public async Task StartGatheringAsync(long userId, long gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);
			_ = (await gathering.Host).LastKnownLocation.Sync();

			// Verify user is host
			Verify(gathering.IsHostedBy(user),
				new UserErrorException(GatheringErrorCode.NOT_HOST));

			// Verify gathering can be started
			Verify(await gathering.IsStartable(),
                new UserErrorException(GatheringErrorCode.CANNOT_START));

            // Try to start gathering
            await Gatherings.UpdateGatheringAsync(gathering.Id, new() { (nameof(CoreGathering.State), GatheringState.Ongoing), (nameof(CoreGathering.StartTime), Time) });
			await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Arrived, Time);

			await gathering.Started();

			// Cancel lingering scheduled notifications
			_ = CancelScheduledNotifications(gathering);
		}

		public async Task TerminateGatheringAsync(long userId, long gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify user is able to end the gathering
			Verify(gathering.IsModifiableBy(user),
				new UserErrorException(GatheringErrorCode.NOT_HOST));

			// Verify gathering is able to be terminated
            Verify(gathering.IsTerminable(),
                new UserErrorException(GatheringErrorCode.CANNOT_END));

            // Try to end gathering
            await Gatherings.TerminateGatheringAsync(gathering.Id, Time);

			// Reshow if hidden
			if (gathering.Visibility == GatheringVisibility.Hidden)
			{
				await Gatherings.UpdateGatheringAsync(gathering.Id, new() { (nameof(CoreGathering.Visibility), GatheringVisibility.Visible) });
			}

			var participants = await gathering.Ended();

			// Update all participants' vectors
			_ = Terminal.AccountDirector.UpdateAllAsync(participants, user => new() { (nameof(CoreUser.Character), user.Character) });

			// Gather no-shows
            var absentUsers = (await gathering.Guests).Except(await gathering.Arrived).Except(await gathering.Left);

            if (gathering.Duration > TimeSpan.FromMinutes(30))
			{
				// Notify no-shows
				_ = User.NotifyAll(CanaryNotification.UserMissedGathering(await gathering.ToGatheringShard()), users: absentUsers.ToArray());

				foreach (var absent in absentUsers)
				{
					await absent.PostTelegram(User.Hollow, TelegramMessage.GatheringMissedAttendee, $"{gathering.Title}");
				}
            }

			// Remove no-shows from the guest list
			foreach (var absent in absentUsers)
			{
				await Gatherings.DeleteUserStateAsync(absent.Id, gathering.Id);
			}

			// Schedule photo reminder for attendees
			_ = User.NotifyAll(CanaryNotification.GatheringUploadClosing(await gathering.ToGatheringShard()), notifyAt: Time + Gathering.MaximumSnapshotLateness * 0.7, users: (await gathering.Left).ToArray());
        }

		public async Task CancelGatheringAsync(long userId, long gatheringId)
		{
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			// Verify gathering has not yet started
			Verify(gathering.IsCancelable(),
                new UserErrorException(GatheringErrorCode.CANNOT_CANCEL_STARTED));

            // Verify user is able to cancel the gathering
            Verify(gathering.IsModifiableBy(user),
                new UserErrorException(GatheringErrorCode.CANNOT_CANCEL_PERMISSION));

            // Try to cancel gathering
            await Gatherings.CancelGatheringAsync(gathering.Id);

            _ = gathering.NotifyActive(CanaryNotification.GatheringCancelled(await gathering.ToGatheringShard()), notifyHost: false);

			// Cancel scheduled notifications
			_ = CancelScheduledNotifications(gathering);
        }

        public async Task ChangeGatheringVisibilityAsync(long userId, long gatheringId, bool hide)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

            // Verify user is gathering host
            Verify(gathering.IsModifiableBy(user),
                new UserErrorException(GatheringErrorCode.CANNOT_EDIT_PERMISSION));

            // Ensure gathering is editable
            Verify(gathering.IsOngoing,
                new UserErrorException(GatheringErrorCode.NOT_STARTED));

            // Ensure gathering is not sealed
            FailIf(gathering.Visibility == GatheringVisibility.Sealed,
                new UserErrorException(GatheringErrorCode.SEALED));

            var visibility = hide ? GatheringVisibility.Hidden : GatheringVisibility.Visible;

			await Gatherings.UpdateGatheringAsync(gathering.Id, new() { (nameof(CoreGathering.Visibility), visibility) });
        }

        public async Task WatchGatheringAsync(long userId, long gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify user is allowed to view gathering
			Verify(await gathering.IsVisibleTo(user),
				new UserErrorException(GatheringErrorCode.CANNOT_WATCH, new { user.AccountStatus }));

			FailIf(gathering.EndTime.HasValue,
				new UserErrorException(GatheringErrorCode.ENDED));

			GatheringBond? userIntention = null;

			try
			{
				userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);
			}
			catch { }

			// Check that user was not kicked
			FailIf(userIntention.HasValue &&
				userIntention.Value.Equals(GatheringBond.Kicked),
				new UserErrorException(GatheringErrorCode.KICKED));

            // Ensure correct state transition
            if (!userIntention.HasValue)
			{
				// Try to add user to the gathering
				await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Watching, Time);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Cannot watch gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task UnwatchGatheringAsync(long userId, long gatheringId)
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
                new UserErrorException(GatheringErrorCode.KICKED));

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

		public async Task JoinGatheringAsync(long userId, long gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);
			_ = user.LastKnownLocation.Sync();

			// Verify user is allowed to join gathering
			Verify(await gathering.IsJoinableBy(user),
                new UserErrorException(GatheringErrorCode.CANNOT_JOIN, new { user.AccountStatus }));

            GatheringBond? previousUserState = null;

            try
            {
                previousUserState = await Gatherings.GetUserStateAsync(userId, gatheringId);
            }
            catch { }

            // Check that user was not kicked
            FailIf(previousUserState.HasValue &&
                previousUserState.Value.Equals(GatheringBond.Kicked),
                new UserErrorException(GatheringErrorCode.KICKED));

            // Check if user is already guest or arrived
            if (previousUserState.HasValue &&
				(previousUserState.Value.Equals(GatheringBond.Guest) ||
                previousUserState.Value.Equals(GatheringBond.Arrived)))
			{
                throw new UserErrorException(GatheringErrorCode.CANNOT_JOIN_GUEST);
            }
            // Check if user has an active gathering conflict
            if (HasAlready(gathering.StartTime))
			{ await ThrowIfUserAtGathering(user); }
			else
			{
				// Check if user has an upcoming conflict
				var conflict = (await user.UpcomingGatherings).Find(e => IsWithin(e.StartTime - gathering.StartTime, HalfHour));
				if (conflict != null)
				{ throw new UserErrorException(GatheringErrorCode.CONFLICT, new { conflict.Id });
                }
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
				await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Guest, Time);

				// Schedule notifications as required
				_ = ScheduleNotificationsForGuest(gathering, user);

				// Notify any companions at gathering
				var activeGuests = (await gathering.Guests).Concat(await gathering.Arrived);
				var userCompanions = await user.Companions;

				var activeCompanions = activeGuests.Where(userCompanions.Contains);

				_ = User.NotifyAll(CanaryNotification.CompanionJoined(user.ToUserShard(), await gathering.ToGatheringShard()), users: activeCompanions.ToArray());
            }

			// Notify host if gathering has already started
			/* TODO?
			if (HasAlready(gathering.StartTime))
			{
				var host = await GetUserAsync(gathering.Host.Id);
				_ = host.Notify(NotificationGroup.GatheringActivity, $"Guest Inbound", $"{user.Name} is joining your gathering.");
			}
			*/
		}

		public async Task CheckInToGatheringAsync(long userId, double latitude, double longitude)
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
                new UserErrorException(GatheringErrorCode.USER_ATTENDING_ELSEWHERE));
            FailIf(nextGathering.Equals(Gathering.None) || !nextGathering.IsOngoing,
                new UserErrorException(GatheringErrorCode.NO_IMMEDIATE));
            // FailIf(!await nextGathering.IsInRange(user),
            //     new InvalidActionException("User is not in range of the gathering."));
            
			await Gatherings.SetUserStateAsync(user.Id, nextGathering.Id, GatheringBond.Arrived, Time);
        }

		public async Task LeaveGatheringAsync(long userId, long gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify user is the host
			FailIf(gathering.IsHostedBy(user),
				new UserErrorException(GatheringErrorCode.CANNOT_LEAVE_HOST));

            // Get the user's current status
            var userIntention = await Gatherings.GetUserStateAsync(userId, gatheringId);

            // Check that user was not kicked
            FailIf(userIntention.HasValue &&
                userIntention.Value.Equals(GatheringBond.Kicked),
                new UserErrorException(GatheringErrorCode.KICKED));

            // Check if user is guest or arrived
            if (userIntention.Equals(GatheringBond.Arrived))
			{
				// Try to remove user from gathering
				await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Left, Time);
			}
			else if (userIntention.Equals(GatheringBond.Guest))
			{
				// Check if user previously left gathering
				if (await gathering.WasAttendedBy(user))
				{
					// TODO This should not create false data.
					await Gatherings.SetUserStateAsync(user.Id, gathering.Id, GatheringBond.Left, Time);
				}
				else
				{
					// Try to remove user from gathering
					await Gatherings.DeleteUserStateAsync(user.Id, gathering.Id);

					// Cancel scheduled notifications
					_ = CancelScheduledNotificationsForGuest(gathering, user);
				}
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not leave gathering, user currently {userIntention.Value} gathering."); }
		}

		public async Task<List<GuestListBondPair>>
			GetGuestListAsync(long userId, long gatheringId)
		{
			var user = await GetUserAsync(userId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Gather
			var allGuests = SelectAsBonds(await gathering.AllUsers,
				user => user.State != GatheringBond.Watching && user.State != GatheringBond.Kicked);

			// Sort
			allGuests.Sort((bond1, bond2) =>
            {
                int bondComparison = GetBondPriority(bond1.Bond).CompareTo(GetBondPriority(bond2.Bond));
                if (bondComparison != 0) return bondComparison;

				return bond1.User.Name.CompareTo(bond2.User.Name);
            });

			// Hide

			// Check if user is host or attendee
			if (gathering.IsModifiableBy(user) || await gathering.WasAttendedBy(user))
            {
				for (int i = 0; i < allGuests.Count; i++)
				{
					User guest = allGuests[i].User;
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
                    User guest = allGuests[i].User;

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
			{ throw new UserErrorException(GatheringErrorCode.CANNOT_VIEW); }

			List<GuestListBondPair> allGuestShards = allGuests
				.ConvertAll(userDetails => new GuestListBondPair(userDetails.User.ToUserShard(), userDetails.Bond));

            return allGuestShards;
		}

		public async Task<List<UserShard>> GetPotentialInviteesAsync(long userId, long gatheringId)
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

		public async Task InviteUserAsync(long inviterId, long inviteeId, long gatheringId)
		{
			var inviter = await GetUserAsync(inviterId);
			var invitee = await GetUserAsync(inviteeId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify inviter has relationship with gathering
			Verify(await gathering.HasUserRelationship(inviter),
				new UserErrorException(GatheringErrorCode.NOT_GUEST));

			// Verify that the invitee can join the gathering
			Verify(await gathering.IsJoinableBy(invitee),
				new UserErrorException(GatheringErrorCode.CANNOT_INVITE_INVALID_INVITEE));

			// Verify that inviter is companions with the invitee
			Verify(await inviter.IsCompanionsWith(invitee),
				new UserErrorException(GatheringErrorCode.CANNOT_INVITE_NEUTRAL));

			_ = invitee.PostTelegram(inviter, TelegramMessage.GatheringInvitation, $"{gathering.Id}");
			_ = invitee.Notify(CanaryNotification.GatheringInvitation(inviter.ToUserShard(), await gathering.ToGatheringShard()));
		}

		public async Task KickUserAsync(long hostId, long targetId, long gatheringId)
		{
			var host = await GetUserAsync(hostId);
			var targetUser = await GetUserAsync(targetId);
			var gathering = await GetGatheringAsync(gatheringId);

			// Verify kicking user is the host
			Verify(gathering.IsHostedBy(host),
				new UserErrorException(GatheringErrorCode.CANNOT_KICK_PERMISSION));

			// Verify gathering is active
			Verify(gathering.IsActive,
				new UserErrorException(GatheringErrorCode.CANNOT_KICK_ARCHIVED));

			// Verify host is not kicking themself
			FailIf(host.Equals(targetUser),
				new UserErrorException(GatheringErrorCode.CANNOT_KICK_SELF));

			// Kick target user from gathering
			await Gatherings.SetUserStateAsync(targetUser.Id, gathering.Id, GatheringBond.Kicked, Time);

			// Remove target user's snapshots from gathering
			foreach (SnapshotShard snapshot in await gathering.Snapshots)
			{
				if (targetUser.Taken(snapshot))
				{ _ = Snapshots.SoftDeleteAsync(snapshot.Id); }
			}

			// Cancel any scheduled notifications
		}

		public async Task<bool> AuthorisedToStart(long userId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			return gathering.IsHostedBy(user) && await gathering.IsStartable();
        }

		public async Task<bool> AuthorisedToJoin(long userId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

			return await user.CanJoin(gathering);
        }

		public async Task<bool> AuthorisedToCheckIn(long userId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var gathering = await GetGatheringAsync(gatheringId);

            return await user.CanCheckIn(gathering);
        }

		public async Task<bool> AuthorisedToUpload(long userId, long gatheringId)
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
				.ConvertAll(userDetails => (User.GetUserAsync(userDetails.UserId).Result, userDetails.State));
		}

		internal async Task<List<(User User, DateTimeOffset Joined, DateTimeOffset? Left)>>
			RequestGuestHistoryAsync(Gathering gathering)
		{
			return (await Gatherings.GetGuestHistoryAsync(gathering.Id))
				.ConvertAll(userDetails => (User.GetUserAsync(userDetails.UserId).Result, userDetails.Joined, userDetails.Left));
		}

		internal async Task<List<GatheringShard>>
			RemoveInaccessibleGatheringsAsync(User user, List<CoreGathering> gatherings)
		{
			List<GatheringShard> accessibleGatherings = new();

			foreach (CoreGathering coreGathering in gatherings)
			{
				Gathering gathering = new(coreGathering);

				if (await user.CanJoin(gathering))
				{ accessibleGatherings.Add(await gathering.ToGatheringShard(user)); }
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
				{ accessibleGatherings.Add(await gathering.ToGatheringShard()); continue; }

				if (gathering.RelativeAngle < maximumAngle)
				{ accessibleGatherings.Add(await gathering.ToGatheringShard()); }
            }

            return accessibleGatherings;
		}

		internal async Task<List<GatheringShard>>
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
					list.Add(await gath.ToGatheringShard());
				}
			}

			return list;
		}

		internal async Task<bool> RequestUserIsAuthorisedGuest(User user, Gathering gathering)
		{
			return await Gatherings.UserIsAuthorizedGuest(user.Id, gathering.Id);
		}

		#endregion

		#region Tools

		private async Task ThrowIfUserAtGathering(User user)
		{
			FailIf(await user.IsAtGathering(),
                new UserErrorException(GatheringErrorCode.USER_ATTENDING_ELSEWHERE, new { (await user.CurrentGathering).Title }));
		}

		private List<(User User, GatheringBond Bond)>
			SelectAsBonds(List<(User User, GatheringBond State)> users, Func<(User User, GatheringBond State), bool> predicate)
		{
			return users.Where(predicate).ToList().ConvertAll(userDetails => (userDetails.User, userDetails.State));
		}

		private (User User, GatheringBond Bond) AsHiddenBondPair(GatheringBond bond)
		{
			return new(User.Hidden, bond);
		}

        private int GetBondPriority(GatheringBond bond)
        {
            return bond switch
            {
                GatheringBond.Arrived => 0, // sorted first
                GatheringBond.Guest => 1,   // sorted next
                GatheringBond.Left => 2,    // sorted last
                _ => 3
            };
        }

		private async Task RescheduleSchedule(Gathering gathering)
		{
            // Cancel scheduled notifications
            await CancelScheduledNotifications(gathering);

            // Reschedule guests and host
            await ScheduleNotifications(gathering);
        }

		private async Task CancelScheduledNotifications(Gathering gathering)
		{
			var (HostSchedule, GuestSchedules) = await Telegrams.GetGatheringNotificationScheduleAsync(gathering.Id);

            // Cancel host notification
            try
            {
				await Terminal.NotificationService.CancelNotification(HostSchedule.GatheringWaitingId);
            }
            catch (Exception e)
            {
                Log.LogError("Was unable to cancel host notification for {gathering}: {error}", gathering.Title, e);
            }

			// Cancel guest notifications
			await CancelScheduledNotificationBatch(gathering, GuestSchedules);

			await Telegrams.ClearGatheringNotificationScheduleAsync(gathering.Id);
		}

        private async Task CancelScheduledNotificationsForGuest(Gathering gathering, User guest)
        {
            var (_, GuestSchedules) = await Telegrams.GetGatheringNotificationScheduleAsync(gathering.Id);

            var schedule = GuestSchedules.Find(s => s.UserId.Equals(guest.Id));

            var batch = GuestSchedules.Where(s =>
                s.GatheringUpcomingId.Equals(schedule.GatheringUpcomingId) || s.GatheringImminentId.Equals(schedule.GatheringImminentId));
            bool isBatchedNotification = batch.Count() > 1;

            // If batched, reschedule everyone
            if (isBatchedNotification)
            {
                // Cancel scheduled notifications
                await CancelScheduledNotifications(gathering);

                // Reschedule guests and host
                await ScheduleNotifications(gathering);
            }
            // else, simple remove
            else
            {
                await CancelScheduledNotificationBatch(gathering, new() { schedule });
            }
        }

        private async Task CancelScheduledNotificationBatch(Gathering gathering, List<GuestNotificationSchedule> schedules)
        {
			// Flatten batches
			// TODO

			// Cancel batches
            foreach (var schedule in schedules)
            {
                var upcomingSync = Terminal.NotificationService.CancelNotification(schedule.GatheringUpcomingId);
                var imminentSync = Terminal.NotificationService.CancelNotification(schedule.GatheringImminentId);

                try
                {
                    await Psijic.Once(upcomingSync, imminentSync);
                }
                catch (Exception e)
                {
                    Log.LogError("Was unable to cancel guest {id} notification for {gathering}: {error}", schedule.UserId, gathering.Title, e);
                }
            }
        }

        private async Task ScheduleNotifications(Gathering gathering)
		{
            // TODO Should make this a bit more intelligent.

            var shard = await gathering.ToGatheringShard();

            // Schedule host
            var waitingNotificationIdSync = (await gathering.Host).Notify(CanaryNotification.GatheringWaiting(shard), shard.StartTime);

			// Schedule guests
            var upcomingIdSync = Task.FromResult("");
            bool scheduleUpcoming = gathering.StartTime - OneHour > Time;

            if (scheduleUpcoming)
            {
                upcomingIdSync = gathering.NotifyGuests(CanaryNotification.GatheringUpcoming(shard, "in an hour"), shard.StartTime - OneHour);
            }

            var imminentIdSync = gathering.NotifyGuests(CanaryNotification.GatheringImminent(shard), shard.StartTime - FifteenMinutes);

			// Await scheduling
			string upcomingNotificationId = await upcomingIdSync, imminentNotificationId = await imminentIdSync;

            var schedules = (await gathering.Guests).Select(guest => (guest.Id, upcomingNotificationId, imminentNotificationId));

			// Track notification ids
			if (await waitingNotificationIdSync != "")
			{ await Telegrams.UpdateGatheringHostNotificationScheduleAsync(gathering.Id, await waitingNotificationIdSync); }

			if (upcomingNotificationId != "" || imminentNotificationId != "")
			{ await Telegrams.UpdateGatheringGuestNotificationSchedulesAsync(gathering.Id, schedules.ToArray()); }
        }

		private async Task ScheduleNotificationsForGuest(Gathering gathering, User guest)
		{
			var shard = await gathering.ToGatheringShard();

            var upcomingIdSync = Task.FromResult("");
            bool scheduleUpcoming = gathering.StartTime - OneHour < Time;

            if (scheduleUpcoming)
            {
                upcomingIdSync = guest.Notify(CanaryNotification.GatheringUpcoming(shard, "in an hour"), shard.StartTime - OneHour);
            }

            var imminentIdSync = guest.Notify(CanaryNotification.GatheringImminent(shard), shard.StartTime - FifteenMinutes);

            // Await scheduling
            string upcomingNotificationId = await upcomingIdSync, imminentNotificationId = await imminentIdSync;

			// Track notification ids
			if (upcomingNotificationId != "" || imminentNotificationId != "")
			{ await Telegrams.UpdateGatheringGuestNotificationSchedulesAsync(gathering.Id, (guest.Id, upcomingNotificationId, imminentNotificationId)); }
        }

        #endregion
    }
}
