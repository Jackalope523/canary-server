using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IEventDatabase
	{
		(string hostID, float latitude, float longitude, string eventType, DateTime startTime) GetEvent(string eventID);
		List<(string eventID, float latitude, float longitude, string eventType)> GetEvents(float latitude, float longitude, int eventLimit = 20);

		void CreateEvent(string hostID, float latitude, float longitude);
		void JoinEvent(string identification, string eventID);
		void LeaveEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<(string userID, string name, string profilePhoto)> GetGuestList(string identification, string eventID);
	}
}
