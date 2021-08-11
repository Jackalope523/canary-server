using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Server.Entities
{
    internal class Event
    {
        #region Properties

        public IList<Participant> Participants => ImmutableList.CreateRange(Participants.ToList());

        public DateTime StartTime { get; init; }

        #endregion

        #region Fields

        public string EventID { get; }
        public string HostID { get; }
        // Location class needed


        private SortedSet<Participant> currentParticipants = new(Participant.CompareID);
        private List<PastParticipant> participantHistory = new();
        // Event Status data type needed

        #endregion

        public Event(string hostID)
        {
            HostID = hostID;
            
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

        public readonly string ID;
        public readonly DateTime JoinedTime;

        public Participant(string userID)
        {
            ID = userID;
            JoinedTime = DateTime.Now.ToUniversalTime();
        }

        public int CompareTo(Participant other)
        {
            return ID.CompareTo(other.ID);
        }
    }

    struct PastParticipant
    {
        public readonly string ID;
        public readonly DateTime JoinedTime;
        public readonly DateTime LeftTime;

        public PastParticipant(Participant participant)
        {
            ID = participant.ID;
            JoinedTime = participant.JoinedTime;
            LeftTime = DateTime.Now.ToUniversalTime();
        }
    }
}
