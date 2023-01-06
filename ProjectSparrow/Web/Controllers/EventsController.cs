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

			List<string> eventList;

            try
            {
                eventList = events.GetPersonalisedEventsInArea(userLocation.Identification, userLocation.Latitude, userLocation.Longitude, userLocation.Distance);
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

			List<string> eventList;

			try
			{
				eventList = events.GetEventsInArea(userLocation.Identification, userLocation.Latitude, userLocation.Longitude, userLocation.Distance);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok(eventList);
		}

        [HttpGet("{eventID}")]
        public IActionResult GetEvent(string eventID)
        {
			if (eventID == null || eventID == string.Empty)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

            try
            {
                events.GetEventInformation("", eventID); // TODO Return relevant information
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
                events.CreateEvent(eventDetails.Identification, eventDetails.Location.Latitude, eventDetails.Location.Longitude); // TODO Add more event relevant information
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

        [HttpDelete("{eventID}")]
        public IActionResult EndEvent([FromBody] EventModel user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				events.EndEvent(user.Identification, user.EventID);
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
				events.JoinEvent(user.Identification, user.EventID);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

		[HttpPut("{eventID}")]
		public IActionResult LeaveEvent([FromBody] EventModel user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				events.LeaveEvent(user.Identification, user.EventID);
			}
			catch
			{
				return BadRequest(EventError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

	}

}
