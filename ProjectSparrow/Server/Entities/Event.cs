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

        public string EventID { get; }
        public string HostID { get; }
        public GeoLocation Location { get; }
        public EventType EventType { get; init; }
        public DateTime StartTime { get; private init; }

        public IList<Participant> Participants => ImmutableList.CreateRange(participantLog.ActiveParticipants.ToList());

        // Event Status data type needed. e.g. gaining popularity

        private readonly ParticipantLog participantLog;

        #endregion

        public Event(string hostID)
        {
            HostID = hostID;

            participantLog = new();
            ParticipantJoined(hostID);

            StartTime = DateTime.UtcNow;
        }

        public void ParticipantJoined(string participantID)
        {
            participantLog.AddParticipant(participantID);
        }

        public void ParticipantLeft(string participantID)
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
