using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Frontier.Manifests;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Twilio.TwiML.Voice;
using Microsoft.Extensions.Hosting;
using Shared;

namespace Frontier.Controllers
{
    [Route("event")]
    [ApiController]
	[Authorize]
    public class EventAgent : ControllerBase
    {
        enum EventError
		{
			MissingInformation,
			CouldNotFindEvent,
			CouldNotCompleteRequest
		}

		IEventOperations events;
		IEtchingOperations etchings;
		IReportOperations reports;
		UserManager<UserShard> userManager;

		public EventAgent(IEventOperations eventOperations,
			IEtchingOperations etchingOperations, IReportOperations reportOperations,
			UserManager<UserShard> identityUserManager)
        {
            events = eventOperations;
			etchings = etchingOperations;
			reports = reportOperations;
			userManager = identityUserManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(ulong eventId)
        {
			EventShard targetEvent;

            try
            {
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
                targetEvent = await events.GetEventInformationAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(targetEvent);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDetailsManifest eventDetails)
        {
            if (eventDetails == null || !ModelState.IsValid)
            {
                return BadRequest(EventError.MissingInformation.ToString());
            }

			EventShard newEvent;

            try
            {
				// Create a new event as the current user
				var user = await GetCurrentUserAsync();
                newEvent = await events.CreateEventAsync(user.Id,
					eventDetails.EventName, eventDetails.EventDescription,
					eventDetails.StartTime,	eventDetails.Latitude, eventDetails.Longitude,
					eventDetails.GroupMinimum, eventDetails.GroupMaximum);
            }
            catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

            return Ok(newEvent);
        }

        [HttpPost("{eventId}/edit")]
        public async Task<IActionResult> EditEvent(ulong eventId, [FromBody] EventEditManifest eventDetails)
		{
			if (eventDetails == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				await events.EditEventAsync(user.Id, eventId,
					eventDescription: eventDetails.EventDescription ?? "",
					isOpen: eventDetails.EventIsOpen);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

        [HttpDelete("{eventId}/edit")]
        public async Task<IActionResult> EndEvent(ulong eventId)
		{
			try
			{
				// End an event as the current user
				var user = await GetCurrentUserAsync();
				await events.EndEventAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
        }

		[HttpPost("{eventId}/watch")]
		public async Task<IActionResult> WatchEvent(ulong eventId)
		{
			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				await events.WatchEventAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("{eventId}/watch")]
		public async Task<IActionResult> UnwatchEvent(ulong eventId)
		{
			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				await events.UnwatchEventAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventId}")]
        public async Task<IActionResult> JoinEvent(ulong eventId)
		{
			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				await events.JoinEventAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("{eventId}")]
		public async Task<IActionResult> LeaveEvent(ulong eventId)
		{
			try
			{
				// Leave an event as the current user
				var user = await GetCurrentUserAsync();
				await events.LeaveEventAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventId}/report")]
		public async Task<IActionResult> ReportEvent(ulong eventId, ulong hostId, [FromBody] EventReportManifest report)
		{
			if (report == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
                await reports.ReportEventAsync(user.Id, eventId, hostId, report.ReportType, report.ReportDetails);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpGet("{eventId}/etchings")]
		public async Task<IActionResult> GetEventEtchings(ulong eventId)
		{
			List<Etching> eventEtchings;

			try
			{
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				eventEtchings = await etchings.GetEventEtchingsAsync(user.Id, eventId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventEtchings);
		}

		[HttpPost("{eventId}/etchings")]
		public async Task<IActionResult> EtchingToEvent(ulong eventId, [FromBody] EventEtchingManifest etching)
		{
			if (etching == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			Etching newEtching;

			try
			{
				var user = await GetCurrentUserAsync();
				newEtching = await etchings.AddEtchingAsync(user.Id, eventId, etching.ImageURL);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(newEtching);
		}

		[HttpPut("{eventId}/etchings")]
		public async Task<IActionResult> RemoveEtching(ulong eventId, ulong etchingId)
		{
			try
			{
				var user = await GetCurrentUserAsync();
				await etchings.RemoveEtchingAsync(user.Id, etchingId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventId}/etchings/{etchingId}")]
		public async Task<IActionResult> RateEtching(ulong eventId, ulong etchingId, [FromBody] AccountRatingManifest details)
		{
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				await etchings.RateEtchingAsync(user.Id, etchingId, details.Rating);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		private async Task<UserShard> GetCurrentUserAsync()
		{
			return await userManager.GetUserAsync(HttpContext.User);
		}
	}

}
