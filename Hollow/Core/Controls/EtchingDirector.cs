using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

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

		public async Task<List<Etching>> GetEventEtchingsAsync(ulong userId, ulong eventId)
        {
            var user = await GetUserAsync(userId);
            var targetEvent = await GetEventAsync(eventId);
            _ = targetEvent.Etchings.Sync();

            // Verify user can see the event
            Try(await targetEvent.WasAttendedBy(user) || targetEvent.IsModifiableBy(user),
                new InvalidEventException("User did not attend event."));

            return await targetEvent.Etchings;
        }

        public async Task<Etching> AddEtchingAsync(ulong userId, ulong eventId, MemoryStream image)
        {
            var userSync = GetUserAsync(userId);
            var targetEventSync = GetEventAsync(eventId);
            var user = await userSync;
            var targetEvent = await targetEventSync;

            await targetEvent.Etched(user);

            // Try to etch
            var etching = await Etchings.AddEtchingAsync(targetEvent.Id, user.Id, Time);

            // Save image
            await Terminal.MediaDirector.UploadImageAsync(user.Id, etching.Id, image);

            return etching;
        }

        public async Task RemoveEtchingAsync(ulong userId, ulong etchingId)
        {
            var userSync = GetUserAsync(userId);
            var etching = await Etchings.GetEtchingAsync(etchingId);
            var eventEtched = await GetEventAsync(etching.EventId);
            var user = await userSync;

            // Verify user owns the etching or can modify the event
            Try(user.Etched(etching) || eventEtched.IsModifiableBy(user),
                new InvalidUserException("User cannot remove etching."));

            await Etchings.RemoveEtchingAsync(etching.Id);
        }

        public async Task RateEtchingAsync(ulong userId, ulong etchingId, UserRating rating)
        {
            var userSync = GetUserAsync(userId);
            var etching = await Etchings.GetEtchingAsync(etchingId);
            var eventEtched = await GetEventAsync(etching.EventId);
            var user = await userSync;

            // Verify user can interact with etching
            Try(await eventEtched.WasAttendedBy(user),
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

        public async Task<Feed>
            GetUserFeedAsync(ulong userId, int depth, int lastDepth)
        {
            var user = await GetUserAsync(userId);
            Dictionary<ulong, EventHeader> eventHeaders = new();

            // Enforce lastDepth < depth
            lastDepth = Math.Min(lastDepth, depth - 1);

            // Retrieve friend-populated event etchings after a specified time excluding previously viewed events
            DateTimeOffset depthCharge = Time - TimeSpan.FromDays(depth);
            DateTimeOffset lastDepthCharge = Time - TimeSpan.FromDays(lastDepth);
            var friendEtchings = await Etchings.GenerateFeedForUserAsync(user.Id, depthCharge, lastDepthCharge);

            // Get the respective event headers for the etchings
            foreach (var etching in friendEtchings)
            {
                var eventId = etching.EventId;

                // Add event header if it does not yet exist
                if (!eventHeaders.ContainsKey(eventId))
                {
                    var etchedEvent = await GetEventAsync(eventId);

                    eventHeaders.Add(eventId, etchedEvent.ToEventHeader(etching.TimeEtched));
                }
                // Update event header active time if etching is more recent
                else if (HappenedBefore(eventHeaders[eventId].LastActiveTime, etching.TimeEtched))
                {
                    eventHeaders[eventId] = new(eventId,
                        eventHeaders[eventId].Name,
                        eventHeaders[eventId].IsActive,
                        etching.TimeEtched,
                        eventHeaders[eventId].Latitude,
                        eventHeaders[eventId].Longitude);
                }
            }

            return new(eventHeaders.Values.ToList(), friendEtchings);
        }

		#endregion

		#region Favours

		internal async Task<List<Etching>> RequestEventEtchingsAsync(Event @event)
            => await Etchings.GetEtchingsForEventAsync(@event.Id);

		#endregion
	}
}

