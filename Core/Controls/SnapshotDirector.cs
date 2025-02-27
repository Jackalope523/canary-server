using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class SnapshotDirector : AbstractDirector, ISnapshotOperations
	{
		#region Initialisation

		public SnapshotDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

        public async Task<GalleryShard> GetGalleryAsync(long userId, long targetId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);
            var gathering = await GetGatheringAsync(gatheringId);

            // Fail if user is blocked
            FailIf(await user.IsBlockedBy(targetUser),
                new UserErrorException(UserErrorCode.CANNOT_VIEW));

            // Fail if user cannot view gathering
            Verify(await user.CanView(gathering),
                new UserErrorException(GatheringErrorCode.CANNOT_VIEW));

            GalleryShard gallery = new(new());

            // Check if user is themself
            if (user.Equals(targetUser))
            {
                // Check if user attended
                if (await gathering.HasOnGuestList(user))
                {
                    gallery = new(await gathering.Snapshots);
                }
                // Check if any companions attended
                else
                {
                    var companionIds = (await user.Companions)
                        .ConvertAll(companion => companion.Id);

                    var companionSnapshots = (await gathering.Snapshots)
                        .Where(snapshot => companionIds.Contains(snapshot.User.Id)).ToList();

                    gallery = new(companionSnapshots);
                }
            }
            // Check if users are companions or attended a common gathering
            else if (await user.IsCompanionsWith(targetUser) ||
                (await gathering.HasOnGuestList(user) && await gathering.HasOnGuestList(targetUser)))
            {
                var targetSnapshots = (await gathering.Snapshots)
                    .Where(snapshot => snapshot.User.Id.Equals(targetUser.Id)).ToList();

                gallery = new(targetSnapshots);
            }

            // Remove strangers if gallery is in pre mode
            if (gathering.IsUpcoming)
            {
                var strangers = await Nests.ReturnStrangerDangerAsync(user.Id, gallery.Snapshots.Select(snapshot => snapshot.User.Id).ToArray());

                // Remove host from strangers
                strangers.Remove(gathering.HostId);

                gallery = new(HideStrangersAsync(gallery.Snapshots, strangers));
            }

            // Remove any snapshots from blocked or blocking users
            GalleryShard filteredGallery = new(await RemoveBlockedSnapshotsAsync(user, gallery.Snapshots));

            return filteredGallery;
        }


        public async Task<SnapshotShard> AddSnapshotAsync(long userId, long gatheringId, MemoryStream image)
        {
            var userSync = GetUserAsync(userId);
            var targetGatheringSync = GetGatheringAsync(gatheringId);
            var user = await userSync;
            var gathering = await targetGatheringSync;

            await user.CanEtch(gathering);

            // Try to etch
            var snapshot = await Snapshots.AddSnapshotAsync(gathering.Id, user.Id, Time);

            try
            {
                // Save image
                await Terminal.MediaDirector.UploadSnapshotAsync(user.Id, snapshot.Id, image);
            }
            catch (Exception ex)
            {
                // If failed, remove snapshot
                await Snapshots.HardDeleteAsync(snapshot.Id);
                throw new UnexpectedFailureException($"Failed to upload snapshot for user {userId} at {gatheringId}.", ex, HollowErrorCode.UPLOAD_FAILED);
            }

            return snapshot;
        }

        public async Task DeleteSnapshotAsync(long userId, long snapshotId)
        {
            var userSync = GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            var gatheringTaken = await GetGatheringAsync(snapshot.GatheringId);
            var user = await userSync;

            // Verify user owns the snapshot or can modify the gathering
            Verify(user.Taken(snapshot) || gatheringTaken.IsModifiableBy(user),
                new UserErrorException(SnapshotErrorCode.CANNOT_DELETE));

            await Snapshots.SoftDeleteAsync(snapshot.Id);
        }

        public async Task AcclaimSnapshotAsync(long userId, long snapshotId, SnapshotAcclaim acclaim)
        {
            var userSync = GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            var gatheringTaken = await GetGatheringAsync(snapshot.GatheringId);
            var user = await userSync;

            // Verify user can interact with snapshot
            Verify(await gatheringTaken.WasAttendedBy(user),
                new UserErrorException(SnapshotErrorCode.CANNOT_INTERACT));

            FailIf(user.Taken(snapshot),
                new UserErrorException(SnapshotErrorCode.CANNOT_INTERACT_SELF));

            // Check action
            if (acclaim == SnapshotAcclaim.Acclaim)
            {
                await Snapshots.AcclaimSnapshotAsync(snapshot.Id, user.Id);
            }
            else
            {
                await Snapshots.DeleteSnapshotAcclaimAsync(snapshot.Id, user.Id);
            }
        }

        public async Task<ColumnShard>
            GetWallAsync(long userId, int depth, int lastDepth)
        {
            var user = await GetUserAsync(userId);
            Dictionary<long, GatheringHeader> gatheringHeaders = new();

            // Enforce lastDepth < depth
            lastDepth = Math.Min(lastDepth, depth - 1);

            // Retrieve companion-populated gathering snapshots after a specified time excluding previously viewed gatherings
            DateTimeOffset depthCharge = Time - TimeSpan.FromDays(depth);
            DateTimeOffset lastDepthCharge = Time - TimeSpan.FromDays(lastDepth);
            var generatedWall = await Snapshots.GenerateColumnForUserAsync(user.Id, depthCharge, lastDepthCharge);

            // Get the respective gathering headers for the snapshots
            foreach (var snapshot in generatedWall)
            {
                var gatheringId = snapshot.GatheringId;

                // Add gathering header if it does not yet exist
                if (!gatheringHeaders.ContainsKey(gatheringId))
                {
                    var etchedGathering = await GetGatheringAsync(gatheringId);

                    gatheringHeaders.Add(gatheringId, etchedGathering.ToGatheringHeader(snapshot.TimeTaken));
                }
                // Update gathering header active time if snapshot is more recent
                else if (HappenedBefore(gatheringHeaders[gatheringId].LastActiveTime, snapshot.TimeTaken))
                {
                    gatheringHeaders[gatheringId] = gatheringHeaders[gatheringId] with { LastActiveTime = snapshot.TimeTaken };
                }
            }

            return new(gatheringHeaders.Values.ToList(), generatedWall);
        }

		#endregion

		#region Favours

		internal async Task<List<SnapshotShard>> RequestGatheringSnapshotsAsync(Gathering gathering)
            => await Snapshots.GetSnapshotsForGatheringAsync(gathering.Id);

		internal async Task<List<SnapshotShard>> RequestVisibleSnapshotsAsync(User user, Gathering gathering)
        {
            // Verify user can see the gathering
            if (!await user.CanView(gathering))
            {
                return new List<SnapshotShard> { };
            }

            _ = gathering.Snapshots.Sync();

            // Determine the level of visibility to the user

            // Check if the user attended the gathering
            if (await gathering.WasAttendedBy(user) || gathering.IsModifiableBy(user))
            {
                return await gathering.Snapshots;
            }
            else
            {
                // Get all companion snapshots
                var companions = await user.Companions;

                var companionSnapshots = (await gathering.Snapshots)
                    .Where(snapshot => companions.Exists(cmp => cmp.Id.Equals(snapshot.User.Id)))
                    .ToList();

                return companionSnapshots;
            }
        }

        internal List<SnapshotShard>
            HideStrangersAsync(List<SnapshotShard> snapshots, List<long> strangers)
        {
            List<SnapshotShard> collection = new();

            foreach (SnapshotShard snapshot in snapshots)
            {
                if (strangers.Contains(snapshot.User.Id))
                {
                    collection.Add(new(snapshot.Id, snapshot.GatheringId,
                        User.Hidden.ToUserShard(),
                        snapshot.TimeTaken, snapshot.Acclaim));
                }
                else
                {
                    collection.Add(snapshot);
                }
            }

            return collection;
        }

        internal async Task<List<SnapshotShard>>
            RemoveBlockedSnapshotsAsync(User user, List<SnapshotShard> snapshots)
        {
            List<SnapshotShard> accessibleSnapshots = new();

            foreach (SnapshotShard snapshot in snapshots)
            {
                User snapshotOwner = await GetUserAsync(snapshot.User.Id);

                // Check if blocking link exists
                if (await user.IsBlocking(snapshotOwner) || await user.IsBlockedBy(snapshotOwner))
                { continue; }

                accessibleSnapshots.Add(snapshot);
            }

            return accessibleSnapshots;
        }

        #endregion
    }
}

