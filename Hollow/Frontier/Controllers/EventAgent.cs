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

        [HttpGet("{eventID}")]
        public async Task<IActionResult> GetEvent(string eventID)
        {
			if (eventID == null)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			EventShard targetEvent;

            try
            {
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
                targetEvent = await events.GetEventInformationAsync(user.Id, eventGUID);
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

        [HttpPost("{eventID}/edit")]
        public async Task<IActionResult> EditEvent(string eventID, [FromBody] EventEditManifest eventDetails)
		{
			if (eventID == null || eventDetails == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				await events.EditEventAsync(user.Id, GetGUID(eventID),
					eventDescription: eventDetails.EventDescription ?? "",
					isOpen: eventDetails.EventIsOpen);
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

		[HttpPost("{eventID}/report")]
		public async Task<IActionResult> ReportEvent(string eventID, string hostID, [FromBody] EventReportManifest report)
		{
			if (string.IsNullOrEmpty(eventID) || report == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
                Guid hostGUID = GetGUID(hostID);
                await reports.ReportEventAsync(user.Id, eventGUID, hostGUID, report.ReportType, report.ReportDetails);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpGet("{eventID}/etchings")]
		public async Task<IActionResult> GetEventEtchings(string eventID)
		{
			if (string.IsNullOrEmpty(eventID))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			List<Etching> eventEtchings;

			try
			{
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);

				eventEtchings = await etchings.GetEventEtchingsAsync(user.Id, eventGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventEtchings);
		}

		[HttpPost("{eventID}/etchings")]
		public async Task<IActionResult> EtchingToEvent(string eventID, [FromBody] EventEtchingManifest etching)
		{
			if (string.IsNullOrEmpty(eventID) || etching == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			Etching newEtching;

			try
			{
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
				newEtching = await etchings.AddEtchingAsync(user.Id, eventGUID, etching.ImageURL);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(newEtching);
		}

		[HttpPut("{eventID}/etchings")]
		public async Task<IActionResult> RemoveEtching(string eventID, string etchingID)
		{
			if (string.IsNullOrEmpty(eventID) || string.IsNullOrEmpty(etchingID))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				Guid etchingGUID = GetGUID(etchingID);
				await etchings.RemoveEtchingAsync(user.Id, etchingGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventID}/etchings/{etchingID}")]
		public async Task<IActionResult> RateEtching(string eventID, string etchingID, [FromBody] AccountRatingManifest details)
		{
			if (string.IsNullOrEmpty(eventID) || details == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				var etchingGUID = GetGUID(etchingID);
				await etchings.RateEtchingAsync(user.Id, etchingGUID, details.Rating);
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
