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

        public async Task<List<Etching>> GetEventEtchingsAsync(Guid userID, Guid eventID)
        {
            var user = await GetUser(userID);
            Event targetEvent = new(eventID);

            // Ensure user can see the event
            if (!await targetEvent.IsAttendedBy(user))
            { throw new InvalidEventException("User did not attend or is not attending event."); }

            var eventEtchings = await Etchings.GetEtchingsForEventAsync(eventID);

            return eventEtchings;
        }

        public async Task<Etching> AddEtchingAsync(Guid userID, Guid eventID, string imageURL)
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
            var userEtching = Etchings.AddEtchingAsync(eventID, userID, DateTimeOffset.UtcNow, imageURL);

            return await userEtching;
        }

        public async Task RemoveEtchingAsync(Guid userID, Guid etchingID)
        {
            var eventEtching = await Etchings.GetEtchingAsync(etchingID);

            // Check if user can delete etching
            if (!eventEtching.UserId.Equals(userID))
            { throw new InvalidUserException("User cannot remove etching."); }

            Etchings.RemoveEtchingAsync(etchingID);
        }

        public async Task RateEtchingAsync(Guid userID, Guid etchingID, UserRating rating)
        {
            User user = new(userID);
            var eventOfEtching = await GetEvent((await Etchings.GetEtchingAsync(etchingID)).EventId);

            // Check if user can interact with etching
            if (!await eventOfEtching.IsAttendedBy(user))
            { throw new InvalidUserException("User cannot interact with etching."); }

            // Check if removing a rating
            if (rating != UserRating.Remove)
            {
                Etchings.RateEtchingAsync(userID, etchingID, rating);
            }
            else
            {
                Etchings.RemoveEtchingRatingAsync(etchingID, userID);
            }
        }

        public async Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(Guid userID, int depth = 0, List<Guid> exclusionList = null)
        {
            User user = new(userID);
            exclusionList ??= new();
            Dictionary<Guid, EventHeader> eventHeaders = new();

            // Retrieve friend-populated event etchings after a specified time excluding previously viewed events
            DateTimeOffset depthCharge = DateTimeOffset.UtcNow - TimeSpan.FromDays(1 + depth);
            var friendEtchings = await Etchings.GenerateFeedForUserAsync(user.Id, depthCharge, exclusionList);

            // Get the respective event headers for the etchings
            foreach (Etching etching in friendEtchings)
            {
                // Add event header if it does not yet exist
                if (!eventHeaders.ContainsKey(etching.EventId))
                {
                    Event etchingEvent = new(await Events.FindEventAsync(etching.EventId));

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


        internal async Task<List<Etching>> GetEventEtchingsAsync(Guid eventID)
        {
            return await Etchings.GetEtchingsForEventAsync(eventID);
        }
    }
}

