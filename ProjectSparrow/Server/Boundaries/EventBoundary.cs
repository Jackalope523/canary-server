using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public record ThinEvent(Guid Id, ThinnerUser Host, string Name, string EventType, DateTime StartTime, float Latitude, float Longitude);
	public record ThinnerEvent(Guid Id, ThinnerUser Host, string EventType, float Latitude, float Longitude);

	public interface IEventDatabase
	{
		ThinEvent FindEvent(Guid Id);
		List<ThinnerEvent> FindEvents(float latitude, float longitude, float distance);

		bool CreateEvent(Guid hostID, string name, string eventType, DateTime startTime, float latitude, float longitude);
		bool AddUserToEvent(Guid userId, Guid eventIDId);
		bool RemoveUserFromEvent(Guid userId, Guid eventIDId);
		bool EndEvent(Guid Id);

		List<ThinnerUser> GetGuestList(Guid Id);
	}

	public interface IEventOperations
	{
		ThinEvent GetEventInformation(string identification, string eventID);
		List<ThinnerEvent> GetEventsInArea(string identification, float latitude, float longitude, float distance);
		List<ThinnerEvent> GetPersonalisedEventsInArea(string identification, float latitude, float longitude, float distance);

		void CreateEvent(string identification, string eventName, string eventType, DateTime startTime, float latitude, float longitude);
		void JoinEvent(string identification, string eventID);
		void LeaveEvent(string identification, string eventID);
		void EndEvent(string identification, string eventID);

		List<ThinnerUser> GetAttendees(string identification, string eventID);
	}
}
