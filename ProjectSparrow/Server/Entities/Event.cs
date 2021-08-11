using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Server.Entities
{
    internal class Event
    {
        #region Variables

        public string EventID { get; }
        public string HostID { get; }
        public DateTime StartTime { get; private init; }

        public IList<Participant> Participants => ImmutableList.CreateRange(Participants.ToList());

        // Event Status data type needed

        private SortedSet<Participant> currentParticipants = new(Participant.CompareID);
        private List<PastParticipant> participantHistory = new();

        #endregion

        public Event(string hostID)
        {
            HostID = hostID;

            StartTime = DateTime.UtcNow;
        }

        public void AddParticipant(string participantID)
        {
            // Processing
        }

        public void RemoveParticipant(string participantID)
        {
            // Processing
        }

    }

    internal struct Participant : IComparable<Participant>
    {
        public static IComparer<Participant> CompareID => Comparer<Participant>.Create((participantA, participantB) => string.Compare(participantA.ID, participantB.ID));

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

    struct PastParticipant
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
