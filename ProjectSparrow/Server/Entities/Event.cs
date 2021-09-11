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

        public IList<Participant> Participants => ImmutableList.CreateRange(currentParticipants.ToList());

        // Event Status data type needed

        /*
         * Idea for new participant tracking/history system.
         * Single list with each participant, past or present.
         * Each participant has an array of tuples representing an 'action' (joined, left) and a timestamp.
         * Can have a parameter that returns all active participants, otherwise just give them the entire dump.
         * This allows for participants to leave and rejoin without object duplication. It also allows for any future actions to be integrated seamlessly.
         */

        private readonly SortedSet<Participant> currentParticipants = new();
        private readonly List<PastParticipant> participantHistory = new();

        #endregion

        public Event(string hostID)
        {
            HostID = hostID;

            Participant host = new(hostID);
            currentParticipants.Add(host);

            StartTime = DateTime.UtcNow;
        }

        public void AddParticipant(string participantID)
        {
            Participant participant = new(participantID);

            currentParticipants.Add(participant);
        }

        public void RemoveParticipant(string participantID)
        {
            Participant participant = currentParticipants.ToList().Find(x => x.ID.Equals(participantID));

            currentParticipants.Remove(participant);

            PastParticipant pastParticipant = new(participant);

            participantHistory.Add(pastParticipant);
        }

    }


    internal readonly struct Participant : IComparable<Participant>
    {
        public string ID { get; }
        public DateTime JoinedTime { get; }

        public Participant(string userID)
        {
            ID = userID;
            JoinedTime = DateTime.UtcNow;
        }

        public int CompareTo(Participant other)
        {
            return ID.CompareTo(other.ID);
        }
    }


    readonly struct PastParticipant
    {
        public string ID { get; }
        public DateTime JoinedTime { get; }
        public DateTime LeftTime { get; }

        public PastParticipant(Participant participant)
        {
            ID = participant.ID;
            JoinedTime = participant.JoinedTime;
            LeftTime = DateTime.UtcNow;
        }
    }
}
