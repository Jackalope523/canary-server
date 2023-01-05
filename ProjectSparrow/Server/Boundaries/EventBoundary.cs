using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IEventDatabase
	{
		void GetEvent(string eventID);
		List<string> GetEvents(float latitude, float longitude, float distance);
		List<string> GetPersonalisedEvents(float latitude, float longitude, float distance);

		void CreateEvent(float latitude, float longitude);
		void JoinEvent(string identification, string eventID);
		void LeaveEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<string> GetGuestList(string identification, string eventID);
	}
}
