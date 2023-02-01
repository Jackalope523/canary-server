using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Web.Models;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using DataAccess.Entities;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
	[Authorize]
    public class EventsController : ControllerBase
    {
        enum EventError
		{
			MissingInformation,
			CouldNotFindEvent,
			CouldNotCompleteRequest
		}

		IEventOperations events;
		UserManager<ThinUser> userManager;

		public EventsController(IEventOperations eventOperations, UserManager<ThinUser> identityUserManager)
        {
            events = eventOperations;
			userManager = identityUserManager;
        }

        [HttpGet("{eventID}")]
        public async Task<IActionResult> GetEvent(string eventID)
        {
			if (eventID == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			ThinEvent targetEvent;

            try
            {
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
                targetEvent = await events.GetEventInformationAsync(user.Id, eventGUID); // TODO Return relevant information
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(targetEvent);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDetailsModel eventDetails)
        {
            if (eventDetails == null || !ModelState.IsValid)
            {
                return BadRequest(EventError.MissingInformation.ToString());
            }

			ThinEvent newEvent;

            try
            {
				// Create a new event as the current user
				var user = await GetCurrentUserAsync();
                newEvent = await events.CreateEventAsync(user.Id,
					eventDetails.EventName, eventDetails.EventType, eventDetails.StartTime,
					eventDetails.Latitude, eventDetails.Longitude);
            }
            catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

            return Ok(newEvent);
        }

        [HttpPost("{eventID}/edit")]
        public async Task<IActionResult> EditEvent(string eventID, [FromBody] EventDetailsModel eventDetails)
		{
			if (eventID == null || eventDetails == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// events.EditEvent(eventDetails.Identification); // TODO Do we even need this?
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

        [HttpDelete("{eventID}/edit")]
        public async Task<IActionResult> EndEvent(string eventID)
		{
			if (eventID == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// End an event as the current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
				await events.EndEventAsync(user.Id, eventGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
        }

		[HttpPost("{eventID}")]
        public async Task<IActionResult> JoinEvent(string eventID)
		{
			if (eventID == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
				await events.JoinEventAsync(user.Id, eventGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("{eventID}")]
		public async Task<IActionResult> LeaveEvent(string eventID)
		{
			if (eventID == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// Leave an event as the current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
				await events.LeaveEventAsync(user.Id, eventGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		private async Task<ThinUser> GetCurrentUserAsync()
		{
			return await userManager.GetUserAsync(HttpContext.User);
		}

		private Guid GetGUID(string id)
		{
			if (!Guid.TryParse(id, out Guid guid))
			{
				throw new ArgumentException("Not a valid GUID.", nameof(id));
			}

			return guid;
		}
	}

}
