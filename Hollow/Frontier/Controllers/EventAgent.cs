using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;

namespace Frontier.Controllers
{
    [Route("event")]
    public class EventAgent : AbstractAgent
	{
		#region Initialisation

		public EventAgent(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IReportOperations reportOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				reportOperations, notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(ulong eventId)
        {
			EventShard targetEvent;

            try
            {
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				ThrowIfUnverified(user);

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
                return BadRequest(HollowError.MissingInformation.ToString());
            }

			EventShard newEvent;

            try
            {
				// Create a new event as the current user
				var user = await GetCurrentUserAsync();
				ThrowIfUnverified(user);

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
				return BadRequest(HollowError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				return BadRequest(HollowError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				return BadRequest(HollowError.MissingInformation.ToString());
			}

			Etching newEtching;

			try
			{
				var user = await GetCurrentUserAsync();
				ThrowIfUnverified(user);

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
				ThrowIfUnverified(user);

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
				return BadRequest(HollowError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				ThrowIfUnverified(user);

				await etchings.RateEtchingAsync(user.Id, etchingId, details.Rating);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		#endregion
	}
}