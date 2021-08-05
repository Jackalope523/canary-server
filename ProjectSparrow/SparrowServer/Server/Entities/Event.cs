using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparrowServer.Server.Entities
{
    public class Event
    {
        #region Variables

        public string EventID { get; init; }
        public string HostID { get; init; }
        // Location class needed
        public DateTime StartTime { get; init; }
        public IList<Participant> Participants { get; } // Head banger
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

    public struct Participant
    {
        public string ID;
        public DateTime JoinedTime;
    }
}
