using System;
using System.Collections.Generic;
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
            User user = new(userId);
            Event targetEvent = new(eventId);
            var etchingsSync = targetEvent.SyncEtchings();

            // Ensure user can see the event
            Try(await targetEvent.WasAttendedBy(user),
                new InvalidEventException("User did not attend event."));

            await etchingsSync;
            return targetEvent.Etchings;
        }

        public async Task<Etching> AddEtchingAsync(ulong userId, ulong eventId, string imageURL)
        {
            var user = await GetUser(userId);
            var targetEvent = await GetEvent(eventId);

            await targetEvent.Etched(user);

            // Try to etch
            var userEtching = Etchings.AddEtching(targetEvent.Id, user.Id, Time, imageURL);

            return userEtching;
        }

        public async Task RemoveEtchingAsync(ulong userId, ulong etchingId)
        {
            User user = new(userId);
            var etching = Etchings.GetEtching(etchingId);
            var eventEtched = await GetEvent(etching.EventId);

            // Check if user owns the etching or can modify the event
            Try(user.Etched(etching) || eventEtched.IsModifiableBy(user),
                new InvalidUserException("User cannot remove etching."));

            Etchings.RemoveEtching(etching.Id);
        }

        public async Task RateEtchingAsync(ulong userId, ulong etchingId, UserRating rating)
        {
            User user = new(userId);
            var etching = Etchings.GetEtching(etchingId);
            var eventEtched = await GetEvent(etching.EventId);

            // Check if user can interact with etching
            Try(await eventEtched.WasAttendedBy(user),
                new InvalidUserException("User cannot interact with etching."));

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                Etchings.RateEtching(user.Id, etching.Id, rating);
            }
            else
            {
                Etchings.RemoveEtchingRating(etching.Id, user.Id);
            }
        }

        public async Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(ulong userId, int depth = 0, List<ulong> exclusionList = null)
        {
            User user = new(userId);
            exclusionList ??= new();
            Dictionary<ulong, EventHeader> eventHeaders = new();

            // Retrieve friend-populated event etchings after a specified time excluding previously viewed events
            DateTimeOffset depthCharge = Time - TimeSpan.FromDays(1 + depth);
            var friendEtchings = Etchings.GenerateFeedForUser(user.Id, depthCharge, exclusionList);

            // Get the respective event headers for the etchings
            foreach (var etching in friendEtchings)
            {
                // Add event header if it does not yet exist
                if (!eventHeaders.ContainsKey(etching.EventId))
                {
                    var etchedEvent = await GetEvent(etching.EventId);

                    eventHeaders.Add(etching.EventId, etchedEvent.ToEventHeader(etching.TimeEtched));
                }
                // Update event header active time if etching is more recent
                else if (HappenedBefore(eventHeaders[etching.EventId].LastActiveTime, etching.TimeEtched))
                {
                    eventHeaders[etching.EventId] = new(etching.EventId,
                        eventHeaders[etching.EventId].Name,
                        eventHeaders[etching.EventId].IsActive,
                        etching.TimeEtched);
                }
            }

            return (depth, eventHeaders.Values.ToList(), friendEtchings);
        }

		#endregion

		#region Favours

		internal async Task<List<Etching>> RequestEventEtchingsAsync(Event @event)
        {
            return Etchings.GetEtchingsForEvent(@event.Id);
        }

		#endregion
	}
}

