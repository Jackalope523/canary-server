using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

namespace Core.Controls
{
	internal class EtchingDirector : AbstractDirector, IEtchingOperations
	{
		public EtchingDirector(CoreTerminal terminal) : base(terminal) { }

        public async Task<List<Etching>> GetEventEtchingsAsync(ulong userId, ulong eventId)
        {
            var user = await GetUser(userId);
            Event targetEvent = new(eventId);

            // Ensure user can see the event
            if (!await targetEvent.WasAttendedBy(user))
            { throw new InvalidEventException("User did not attend or is not attending event."); }

            var eventEtchings = Etchings.GetEtchingsForEvent(eventId);

            return eventEtchings;
        }

        public async Task<Etching> AddEtchingAsync(ulong userId, ulong eventId, string imageURL)
        {
            User user = new(userId);
            var targetEvent = await GetEvent(eventId);

            // Ensure the user can etching to the event
            if (!await targetEvent.WasAttendedBy(user))
            { throw new InvalidEventException("User did not attend event."); }

            // Ensure etching is added within a day of event ending
            if (targetEvent.EndTime.HasValue && targetEvent.EndTime + TimeSpan.FromDays(1) < DateTimeOffset.UtcNow)
            { throw new InvalidEventException("Event has already ended."); }

            // Try to etching
            var userEtching = Etchings.AddEtching(eventId, userId, DateTimeOffset.UtcNow, imageURL);

            return userEtching;
        }

        public async Task RemoveEtchingAsync(ulong userId, ulong etchingId)
        {
            var eventEtching = Etchings.GetEtching(etchingId);

            // Check if user can delete etching
            if (!eventEtching.UserId.Equals(userId))
            { throw new InvalidUserException("User cannot remove etching."); }

            Etchings.RemoveEtching(etchingId);
        }

        public async Task RateEtchingAsync(ulong userId, ulong etchingId, UserRating rating)
        {
            User user = new(userId);
            var eventOfEtching = await GetEvent(Etchings.GetEtching(etchingId).EventId);

            // Check if user can interact with etching
            if (!await eventOfEtching.WasAttendedBy(user))
            { throw new InvalidUserException("User cannot interact with etching."); }

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                Etchings.RateEtching(userId, etchingId, rating);
            }
            else
            {
                Etchings.RemoveEtchingRating(etchingId, userId);
            }
        }

        public async Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(ulong userId, int depth = 0, List<ulong> exclusionList = null)
        {
            User user = new(userId);
            exclusionList ??= new();
            Dictionary<ulong, EventHeader> eventHeaders = new();

            // Retrieve friend-populated event etchings after a specified time excluding previously viewed events
            DateTimeOffset depthCharge = DateTimeOffset.UtcNow - TimeSpan.FromDays(1 + depth);
            var friendEtchings = Etchings.GenerateFeedForUser(user.Id, depthCharge, exclusionList);

            // Get the respective event headers for the etchings
            foreach (Etching etching in friendEtchings)
            {
                // Add event header if it does not yet exist
                if (!eventHeaders.ContainsKey(etching.EventId))
                {
                    Event etchingEvent = new(Events.FindEvent(etching.EventId));

                    eventHeaders.Add(etching.EventId, etchingEvent.ToEventHeader(etching.TimeEtched));
                }
                // Update event header active time if etching is more recent
                else if (eventHeaders[etching.EventId].LastActiveTime < etching.TimeEtched)
                {
                    eventHeaders[etching.EventId] = new(etching.EventId,
                        eventHeaders[etching.EventId].Name,
                        eventHeaders[etching.EventId].IsActive,
                        etching.TimeEtched);
                }
            }

            return (depth, eventHeaders.Values.ToList(), friendEtchings);
        }


        internal async Task<List<Etching>> GetEventEtchingsAsync(ulong eventId)
        {
            return Etchings.GetEtchingsForEvent(eventId);
        }
    }
}

