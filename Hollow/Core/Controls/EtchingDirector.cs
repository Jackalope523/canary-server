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

        public async Task<List<Etching>> GetEventEtchingsAsync(ulong userID, ulong eventID)
        {
            var user = await GetUser(userID);
            Event targetEvent = new(eventID);

            // Ensure user can see the event
            if (!await targetEvent.IsAttendedBy(user))
            { throw new InvalidEventException("User did not attend or is not attending event."); }

            var eventEtchings = Etchings.GetEtchingsForEvent(eventID);

            return eventEtchings;
        }

        public async Task<Etching> AddEtchingAsync(ulong userID, ulong eventID, string imageURL)
        {
            User user = new(userID);
            var targetEvent = await GetEvent(eventID);

            // Ensure the user can etching to the event
            if (!await targetEvent.IsAttendedBy(user))
            { throw new InvalidEventException("User is not attending event."); }

            // Ensure event is still running
            if (targetEvent.EndTime.HasValue)
            { throw new InvalidEventException("Event has already ended."); }

            // Try to etching
            var userEtching = Etchings.AddEtching(eventID, userID, DateTimeOffset.UtcNow, imageURL);

            return userEtching;
        }

        public async Task RemoveEtchingAsync(ulong userID, ulong etchingID)
        {
            var eventEtching = Etchings.GetEtching(etchingID);

            // Check if user can delete etching
            if (!eventEtching.UserId.Equals(userID))
            { throw new InvalidUserException("User cannot remove etching."); }

            Etchings.RemoveEtching(etchingID);
        }

        public async Task RateEtchingAsync(ulong userID, ulong etchingID, UserRating rating)
        {
            User user = new(userID);
            var eventOfEtching = await GetEvent(Etchings.GetEtching(etchingID).EventId);

            // Check if user can interact with etching
            if (!await eventOfEtching.IsAttendedBy(user))
            { throw new InvalidUserException("User cannot interact with etching."); }

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                Etchings.RateEtching(userID, etchingID, rating);
            }
            else
            {
                Etchings.RemoveEtchingRating(etchingID, userID);
            }
        }

        public async Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(ulong userID, int depth = 0, List<ulong> exclusionList = null)
        {
            User user = new(userID);
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


        internal async Task<List<Etching>> GetEventEtchingsAsync(ulong eventID)
        {
            return Etchings.GetEtchingsForEvent(eventID);
        }
    }
}

