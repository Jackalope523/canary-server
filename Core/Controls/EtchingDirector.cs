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
    internal class EtchingDirector : AbstractDirector, IEtchingOperations
	{
		#region Initialisation

		public EtchingDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<List<EtchingShard>> GetGatheringEtchingsAsync(ulong userId, ulong gatheringId)
        {
            var user = await GetUserAsync(userId);
            var targetGathering = await GetGatheringAsync(gatheringId);
            _ = targetGathering.Etchings.Sync();

            // Verify user can see the gathering
            Try(await targetGathering.WasAttendedBy(user) || targetGathering.IsModifiableBy(user),
                new InvalidGatheringException("User did not attend gathering."));

            return await targetGathering.Etchings;
        }

        public async Task<EtchingShard> AddEtchingAsync(ulong userId, ulong gatheringId, MemoryStream image)
        {
            var userSync = GetUserAsync(userId);
            var targetGatheringSync = GetGatheringAsync(gatheringId);
            var user = await userSync;
            var targetGathering = await targetGatheringSync;

            await user.CanEtch(targetGathering);

            // Try to etch
            var etching = await Etchings.AddEtchingAsync(targetGathering.Id, user.Id, Time);

            // Save image
            await Terminal.MediaDirector.UploadImageAsync(user.Id, etching.Id, image);

            return etching;
        }

        public async Task RemoveEtchingAsync(ulong userId, ulong etchingId)
        {
            var userSync = GetUserAsync(userId);
            var etching = await Etchings.GetEtchingAsync(etchingId);
            var gatheringEtched = await GetGatheringAsync(etching.GatheringId);
            var user = await userSync;

            // Verify user owns the etching or can modify the gathering
            Try(user.Etched(etching) || gatheringEtched.IsModifiableBy(user),
                new InvalidUserException("User cannot remove etching."));

            await Etchings.RemoveEtchingAsync(etching.Id);
        }

        public async Task RateEtchingAsync(ulong userId, ulong etchingId, UserRating rating)
        {
            var userSync = GetUserAsync(userId);
            var etching = await Etchings.GetEtchingAsync(etchingId);
            var gatheringEtched = await GetGatheringAsync(etching.GatheringId);
            var user = await userSync;

            // Verify user can interact with etching
            Try(await gatheringEtched.WasAttendedBy(user),
                new InvalidUserException("User cannot interact with etching."));

            Fail(user.Etched(etching),
                new InvalidUserException("User cannot rate their own etching."));

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                await Etchings.RateEtchingAsync(etching.Id, user.Id, rating);
            }
            else
            {
                await Etchings.RemoveEtchingRatingAsync(etching.Id, user.Id);
            }
        }

        public async Task<FeedShard>
            GetUserFeedAsync(ulong userId, int depth, int lastDepth)
        {
            var user = await GetUserAsync(userId);
            Dictionary<ulong, GatheringHeader> gatheringHeaders = new();

            // Enforce lastDepth < depth
            lastDepth = Math.Min(lastDepth, depth - 1);

            // Retrieve friend-populated gathering etchings after a specified time excluding previously viewed gatherings
            DateTimeOffset depthCharge = Time - TimeSpan.FromDays(depth);
            DateTimeOffset lastDepthCharge = Time - TimeSpan.FromDays(lastDepth);
            var friendEtchings = await Etchings.GenerateFeedForUserAsync(user.Id, depthCharge, lastDepthCharge);

            // Get the respective gathering headers for the etchings
            foreach (var etching in friendEtchings)
            {
                var gatheringId = etching.GatheringId;

                // Add gathering header if it does not yet exist
                if (!gatheringHeaders.ContainsKey(gatheringId))
                {
                    var etchedGathering = await GetGatheringAsync(gatheringId);

                    gatheringHeaders.Add(gatheringId, etchedGathering.ToGatheringHeader(etching.TimeEtched));
                }
                // Update gathering header active time if etching is more recent
                else if (HappenedBefore(gatheringHeaders[gatheringId].LastActiveTime, etching.TimeEtched))
                {
                    gatheringHeaders[gatheringId] = new(gatheringId,
                        gatheringHeaders[gatheringId].Name,
                        gatheringHeaders[gatheringId].IsActive,
                        etching.TimeEtched,
                        gatheringHeaders[gatheringId].Latitude,
                        gatheringHeaders[gatheringId].Longitude);
                }
            }

            return new(gatheringHeaders.Values.ToList(), friendEtchings);
        }

		#endregion

		#region Favours

		internal async Task<List<EtchingShard>> RequestGatheringEtchingsAsync(Gathering @gathering)
            => await Etchings.GetEtchingsForGatheringAsync(@gathering.Id);

		#endregion
	}
}

