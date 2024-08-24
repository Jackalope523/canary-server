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

		public async Task<List<SnapshotShard>> GetGatheringSnapshotsAsync(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
            var targetGathering = await GetGatheringAsync(gatheringId);

            return await RequestVisibleSnapshotsAsync(user, targetGathering);
        }

        public async Task<SnapshotShard> AddSnapshotAsync(ulong userId, ulong gatheringId, MemoryStream image)
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
            catch
            {
                // If failed, remove snapshot
                await Snapshots.DeleteSnapshotAsync(snapshot.Id);
                throw new UnexpectedFailureException("Image upload failed.");
            }

            return snapshot;
        }

        public async Task DeleteSnapshotAsync(ulong userId, ulong snapshotId)
        {
            var userSync = GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            var gatheringTaken = await GetGatheringAsync(snapshot.GatheringId);
            var user = await userSync;

            // Verify user owns the snapshot or can modify the gathering
            Verify(user.Taken(snapshot) || gatheringTaken.IsModifiableBy(user),
                new InvalidUserException("User cannot remove snapshot."));

            await Snapshots.DeleteSnapshotAsync(snapshot.Id);
        }

        public async Task AcclaimSnapshotAsync(ulong userId, ulong snapshotId, SnapshotAcclaim acclaim)
        {
            var userSync = GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            var gatheringTaken = await GetGatheringAsync(snapshot.GatheringId);
            var user = await userSync;

            // Verify user can interact with snapshot
            Verify(await gatheringTaken.WasAttendedBy(user),
                new InvalidUserException("User cannot interact with snapshot."));

            FailIf(user.Taken(snapshot),
                new InvalidUserException("User cannot rate their own snapshot."));

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
            GetUserColumnAsync(ulong userId, int depth, int lastDepth)
        {
            var user = await GetUserAsync(userId);
            Dictionary<ulong, GatheringHeader> gatheringHeaders = new();

            // Enforce lastDepth < depth
            lastDepth = Math.Min(lastDepth, depth - 1);

            // Retrieve companion-populated gathering snapshots after a specified time excluding previously viewed gatherings
            DateTimeOffset depthCharge = Time - TimeSpan.FromDays(depth);
            DateTimeOffset lastDepthCharge = Time - TimeSpan.FromDays(lastDepth);
            var companionSnapshots = await Snapshots.GenerateColumnForUserAsync(user.Id, depthCharge, lastDepthCharge);

            // Get the respective gathering headers for the snapshots
            foreach (var snapshot in companionSnapshots)
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
                    gatheringHeaders[gatheringId] = new(gatheringId,
                        gatheringHeaders[gatheringId].Name,
                        gatheringHeaders[gatheringId].Time,
                        gatheringHeaders[gatheringId].IsActive,
                        snapshot.TimeTaken,
                        gatheringHeaders[gatheringId].FriendlyLocation);
                }
            }

            return new(gatheringHeaders.Values.ToList(), companionSnapshots);
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

		#endregion
	}
}

