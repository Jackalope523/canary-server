using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Web.Models;
using Web.Models.Utilities;
using System.Net;
using System.Runtime.CompilerServices;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        enum EventError
		{
			MissingInformation,
			CouldNotFindEvent,
			CouldNotCompleteRequest
		}

		IEventOperations events;

        public EventsController(IEventOperations eventOperations)
        {
            events = eventOperations;
        }

        [HttpGet]
        public IActionResult GetEventsList([FromBody] GeoLocation userLocation)
        {
			if (userLocation == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			List<ThinnerEvent> eventList;

            try
            {
                eventList = events.GetPersonalisedEventsInArea(userLocation.UserID, userLocation.Latitude, userLocation.Longitude, userLocation.Distance);
            }
            catch
            {
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}
			
			return Ok(eventList);
        }

        [HttpGet("all")]
        public IActionResult GetAllEvents([FromBody] GeoLocation userLocation)
		{
			if (userLocation == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			List<ThinnerEvent> eventList;

			try
			{
				eventList = events.GetEventsInArea(userLocation.UserID, userLocation.Latitude, userLocation.Longitude, userLocation.Distance);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok(eventList);
		}

        [HttpGet("{eventID}")]
        public IActionResult GetEvent([FromBody] EventModel info)
        {
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

            try
            {
                events.GetEventInformation(info.UserID, info.EventID); // TODO Return relevant information
            }
            catch
			{
				return BadRequest(EventError.CouldNotFindEvent.ToString());
			}

			return Ok();
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] EventDetailsModel eventDetails)
        {
            if (eventDetails == null || !ModelState.IsValid)
            {
                return BadRequest(EventError.MissingInformation.ToString());
            }

            try
            {
                events.CreateEvent(eventDetails.UserID,
					eventDetails.EventName, eventDetails.EventType, eventDetails.StartTime,
					eventDetails.Location.Latitude, eventDetails.Location.Longitude);
            }
            catch
            {
                return BadRequest(EventError.CouldNotCompleteRequest.ToString());
            }

            return Ok();
        }

        [HttpPut("{eventID}")]
        public IActionResult EditEvent([FromBody] EventDetailsModel eventDetails)
		{
			if (eventDetails == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// events.EditEvent(eventDetails.Identification); // TODO Do we even need this?
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

        [HttpDelete()]
        public IActionResult EndEvent([FromBody] EventModel user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				events.EndEvent(user.UserID, user.EventID);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
        }

		[HttpPost("{eventID}")]
        public IActionResult JoinEvent([FromBody] EventModel user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				events.JoinEvent(user.UserID, user.EventID);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

		[HttpDelete("{eventID}")]
		public IActionResult LeaveEvent([FromBody] EventModel user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				events.LeaveEvent(user.UserID, user.EventID);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

	}

}
