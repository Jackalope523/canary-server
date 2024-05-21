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
            _ = targetGathering.Snapshots.Sync();

            // Verify user can see the gathering
            Try(await targetGathering.WasAttendedBy(user) || targetGathering.IsModifiableBy(user),
                new InvalidGatheringException("User did not attend gathering."));

            return await targetGathering.Snapshots;
        }

        public async Task<SnapshotShard> AddSnapshotAsync(ulong userId, ulong gatheringId, MemoryStream image)
        {
            var userSync = GetUserAsync(userId);
            var targetGatheringSync = GetGatheringAsync(gatheringId);
            var user = await userSync;
            var targetGathering = await targetGatheringSync;

            await user.CanEtch(targetGathering);

            // Try to etch
            var snapshot = await Snapshots.AddSnapshotAsync(targetGathering.Id, user.Id, Time);

            // Save image
            await Terminal.MediaDirector.UploadImageAsync(user.Id, snapshot.Id, image);

            return snapshot;
        }

        public async Task RemoveSnapshotAsync(ulong userId, ulong snapshotId)
        {
            var userSync = GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            var gatheringEtched = await GetGatheringAsync(snapshot.GatheringId);
            var user = await userSync;

            // Verify user owns the snapshot or can modify the gathering
            Try(user.Etched(snapshot) || gatheringEtched.IsModifiableBy(user),
                new InvalidUserException("User cannot remove snapshot."));

            await Snapshots.RemoveSnapshotAsync(snapshot.Id);
        }

        public async Task AcclaimSnapshotAsync(ulong userId, ulong snapshotId, UserRating rating)
        {
            var userSync = GetUserAsync(userId);
            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            var gatheringEtched = await GetGatheringAsync(snapshot.GatheringId);
            var user = await userSync;

            // Verify user can interact with snapshot
            Try(await gatheringEtched.WasAttendedBy(user),
                new InvalidUserException("User cannot interact with snapshot."));

            Fail(user.Etched(snapshot),
                new InvalidUserException("User cannot rate their own snapshot."));

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                await Snapshots.AcclaimSnapshotAsync(snapshot.Id, user.Id, rating);
            }
            else
            {
                await Snapshots.RemoveSnapshotAcclaimAsync(snapshot.Id, user.Id);
            }
        }

        public async Task<FeedShard>
            GetUserFeedAsync(ulong userId, int depth, int lastDepth)
        {
            var user = await GetUserAsync(userId);
            Dictionary<ulong, GatheringHeader> gatheringHeaders = new();

            // Enforce lastDepth < depth
            lastDepth = Math.Min(lastDepth, depth - 1);

            // Retrieve companion-populated gathering snapshots after a specified time excluding previously viewed gatherings
            DateTimeOffset depthCharge = Time - TimeSpan.FromDays(depth);
            DateTimeOffset lastDepthCharge = Time - TimeSpan.FromDays(lastDepth);
            var companionSnapshots = await Snapshots.GenerateFeedForUserAsync(user.Id, depthCharge, lastDepthCharge);

            // Get the respective gathering headers for the snapshots
            foreach (var snapshot in companionSnapshots)
            {
                var gatheringId = snapshot.GatheringId;

                // Add gathering header if it does not yet exist
                if (!gatheringHeaders.ContainsKey(gatheringId))
                {
                    var etchedGathering = await GetGatheringAsync(gatheringId);

                    gatheringHeaders.Add(gatheringId, etchedGathering.ToGatheringHeader(snapshot.TimeEtched));
                }
                // Update gathering header active time if snapshot is more recent
                else if (HappenedBefore(gatheringHeaders[gatheringId].LastActiveTime, snapshot.TimeEtched))
                {
                    gatheringHeaders[gatheringId] = new(gatheringId,
                        gatheringHeaders[gatheringId].Name,
                        gatheringHeaders[gatheringId].IsActive,
                        snapshot.TimeEtched,
                        gatheringHeaders[gatheringId].Latitude,
                        gatheringHeaders[gatheringId].Longitude);
                }
            }

            return new(gatheringHeaders.Values.ToList(), companionSnapshots);
        }

		#endregion

		#region Favours

		internal async Task<List<SnapshotShard>> RequestGatheringSnapshotsAsync(Gathering @gathering)
            => await Snapshots.GetSnapshotsForGatheringAsync(@gathering.Id);

		#endregion
	}
}

