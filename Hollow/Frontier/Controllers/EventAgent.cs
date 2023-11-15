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
        public async Task<IActionResult> GetEvent(string eventId)
        {
			if (eventId == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			EventShard targetEvent;

            try
            {
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
                targetEvent = await events.GetEventInformationAsync(user.Id, eventUlong);
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
        public async Task<IActionResult> EditEvent(string eventId, [FromBody] EventEditManifest eventDetails)
		{
			if (eventId == null || eventDetails == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				await events.EditEventAsync(user.Id, GetId(eventId),
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
        public async Task<IActionResult> EndEvent(string eventId)
		{
			if (eventId == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// End an event as the current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
				await events.EndEventAsync(user.Id, eventUlong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
        }

		[HttpPost("{eventId}/watch")]
		public async Task<IActionResult> WatchEvent(string eventId)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
				await events.WatchEventAsync(user.Id, eventUlong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("{eventId}/watch")]
		public async Task<IActionResult> UnwatchEvent(string eventId)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
				await events.UnwatchEventAsync(user.Id, eventUlong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventId}")]
        public async Task<IActionResult> JoinEvent(string eventId)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// Join an event as the current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
				await events.JoinEventAsync(user.Id, eventUlong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("{eventId}")]
		public async Task<IActionResult> LeaveEvent(string eventId)
		{
			if (eventId == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				// Leave an event as the current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
				await events.LeaveEventAsync(user.Id, eventUlong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventId}/report")]
		public async Task<IActionResult> ReportEvent(string eventId, string hostId, [FromBody] EventReportManifest report)
		{
			if (string.IsNullOrEmpty(eventId) || report == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
                ulong hostulong = GetId(hostId);
                await reports.ReportEventAsync(user.Id, eventUlong, hostulong, report.ReportType, report.ReportDetails);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpGet("{eventId}/etchings")]
		public async Task<IActionResult> GetEventEtchings(string eventId)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			List<Etching> eventEtchings;

			try
			{
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);

				eventEtchings = await etchings.GetEventEtchingsAsync(user.Id, eventUlong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventEtchings);
		}

		[HttpPost("{eventId}/etchings")]
		public async Task<IActionResult> EtchingToEvent(string eventId, [FromBody] EventEtchingManifest etching)
		{
			if (string.IsNullOrEmpty(eventId) || etching == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			Etching newEtching;

			try
			{
				var user = await GetCurrentUserAsync();
				ulong eventUlong = GetId(eventId);
				newEtching = await etchings.AddEtchingAsync(user.Id, eventUlong, etching.ImageURL);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(newEtching);
		}

		[HttpPut("{eventId}/etchings")]
		public async Task<IActionResult> RemoveEtching(string eventId, string etchingId)
		{
			if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(etchingId))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				ulong etchingulong = GetId(etchingId);
				await etchings.RemoveEtchingAsync(user.Id, etchingulong);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventId}/etchings/{etchingId}")]
		public async Task<IActionResult> RateEtching(string eventId, string etchingId, [FromBody] AccountRatingManifest details)
		{
			if (string.IsNullOrEmpty(eventId) || details == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				var etchingulong = GetId(etchingId);
				await etchings.RateEtchingAsync(user.Id, etchingulong, details.Rating);
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

		private ulong GetId(string id)
		{
			if (!ulong.TryParse(id, out ulong parsedId))
			{
				throw new ArgumentException("Not a valid ulong.", nameof(id));
			}

			return parsedId;
		}
	}

}
