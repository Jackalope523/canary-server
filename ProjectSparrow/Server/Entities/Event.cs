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

        public IList<Participant> Participants => ImmutableList.CreateRange(participantLog.activeParticipants.ToList());

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
            participantLog.RemoveParticipant(participantID);
		}

	}

	internal class ParticipantLog
	{
        private Dictionary<string, LinkedList<ParticipantAction>> log;

        public IList<Participant> activeParticipants => null; // TODO

        public ParticipantLog()
		{
            log = new Dictionary<string, LinkedList<ParticipantAction>>();
		}

        public void AddParticipant(string participantID)
		{
            if (!log.ContainsKey(participantID))
			{
                log.Add(participantID, new LinkedList<ParticipantAction>());
			}

            log[participantID].AddLast(new ParticipantAction(PerformedAction.Joined));
		}

        public void RemoveParticipant(string participantID)
        {
            if (log.ContainsKey(participantID))
            {
                log[participantID].AddLast(new ParticipantAction(PerformedAction.Left));
            }
        }

        public float ParticipantActivityRatio(float timeInMinutes = 15)
		{
            // TODO
            // Returns 1 if participants left to participants joined is equal. <1 if more left, >1 if more joined.
            return 0;
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

    
    internal readonly struct ParticipantAction
	{
        public DateTime Time { get; }
        public PerformedAction Type { get; }

        public ParticipantAction(PerformedAction actionType)
		{
            Time = DateTime.Now;
            Type = actionType;
		}
	}


    internal enum PerformedAction { Joined, Left }
}
