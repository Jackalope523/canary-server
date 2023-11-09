using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    internal class ParticipantLog
    {
        public IList<Participant> ActiveParticipants => GetActiveParticipants();


        private Dictionary<ulong, LinkedList<ParticipantAction>> log;

        private IList<Participant> cachedActiveParticipants;
        private bool participantsIsStale = true;


        public ParticipantLog()
        {
            log = new Dictionary<ulong, LinkedList<ParticipantAction>>();
        }

        public void AddParticipant(ulong participantID)
        {
            participantsIsStale = true;

            if (!log.ContainsKey(participantID))
            {
                log.Add(participantID, new LinkedList<ParticipantAction>());
            }

            log[participantID].AddLast(new ParticipantAction(PerformedAction.Joined));
        }

        public void RemoveParticipant(ulong participantID)
        {
            participantsIsStale = true;

            if (log.ContainsKey(participantID))
            {
                log[participantID].AddLast(new ParticipantAction(PerformedAction.Left));
            }
            else
            {
                // TODO Log error. Could be concurrency or internal logic issue.
            }

        }

        public int ParticipantsPerformedActionWithin(PerformedAction action, float timeInMinutes = 15)
        {
            int count = 0;

            var currentTime = DateTime.Now;

            foreach (ulong participant in log.Keys)
            {
                ParticipantAction lastAction = log[participant].Last.Value;

                if (lastAction.Type == action && currentTime - lastAction.Time < TimeSpan.FromMinutes(timeInMinutes))
                {
                    count += 1;
                }
            }

            return count;
        }

        public float ParticipantJoinRatio(float timeInMinutes = 15)
        {
            float countJoined = 0, countLeft = 0;

            var currentTime = DateTime.Now;

            foreach (ulong participant in log.Keys)
            {
                ParticipantAction lastAction = log[participant].Last.Value;

                if (currentTime - lastAction.Time < TimeSpan.FromMinutes(timeInMinutes))
                {
                    if (lastAction.Type == PerformedAction.Joined)
                    {
                        countJoined += 1;
                    }
                    else
                    {
                        countLeft += 1;
                    }
                }
            }

            return countJoined / countLeft;
        }

        private IList<Participant> GetActiveParticipants()
        {
            if (!participantsIsStale)
            {
                return cachedActiveParticipants;
            }

            participantsIsStale = false;

            List<Participant> activeParticipants = new List<Participant>();

            foreach (ulong participant in log.Keys)
            {
                ParticipantAction lastAction = log[participant].Last.Value;

                if (lastAction.Type == PerformedAction.Joined)
                {
                    activeParticipants.Add(new Participant(participant, lastAction.Time));
                }
            }

            cachedActiveParticipants = activeParticipants;

            return activeParticipants;
        }
    }


    internal readonly struct Participant : IComparable<Participant>
    {
        public ulong ID { get; }
        public DateTime JoinedTime { get; }

        public Participant(ulong userID)
        {
            ID = userID;
            JoinedTime = DateTime.UtcNow;
        }

        public Participant(ulong userID, DateTime timeJoined)
        {
            ID = userID;
            JoinedTime = timeJoined;
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
