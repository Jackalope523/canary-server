using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Shared;

namespace Server.Entities
{
    internal class Event
    {
        #region Variables

        public Guid EventID { get; }
        public User Host { get; }
        public GeoLocation Location { get; }
        public EventType EventType { get; init; }
        public DateTime StartTime { get; private init; }

        public IList<Participant> Participants => ImmutableList.CreateRange(participantLog.ActiveParticipants.ToList());

        // Event Status data type needed. e.g. gaining popularity

        private readonly ParticipantLog participantLog;

        #endregion

        public Event(User eventHost)
        {
            Host = eventHost;

            participantLog = new();
            ParticipantJoined(Host.Id);

            StartTime = DateTime.UtcNow;
        }

        public void ParticipantJoined(Guid participantID)
        {
            participantLog.AddParticipant(participantID);
        }

        public void ParticipantLeft(Guid participantID)
        {
            // TODO Host leaves, too few participants, only host, etc


            participantLog.RemoveParticipant(participantID);
        }

        public bool TransferOwnership()
        {
            // TODO

            return true;
        }

    }
}
