using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace SparrowServer.Server.Entities
{
    public class Event
    {
        #region Variables

        public string EventID { get; init; }
        public string HostID { get; init; }
        // Location class needed
        public DateTime StartTime { get; init; }

        public IList<Participant> Participants => ImmutableList.CreateRange(Participants.ToList());

        private SortedSet<Participant> currentParticipants = new(Participant.CompareID);
        private List<PastParticipant> participantHistory = new();
        // Event Status data type needed

        #endregion


        public void AddParticipant(string participantID)
        {
            // Processing
        }

        public void RemoveParticipant(string participantID)
        {
            // Processing
        }

    }

    public struct Participant : IComparable<Participant>
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
