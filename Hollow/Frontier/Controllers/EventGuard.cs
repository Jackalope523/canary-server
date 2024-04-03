using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;


namespace Frontier.Controllers
{
    [Route("event")]
    public class EventGuard : AbstractGuard
	{
		#region Initialisation

		public EventGuard(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, IMediaOperations mediaOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				disciplineOperations, mediaOperations,
				notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(ulong eventId)
        {
			return await Execute(async user =>
			{
				// Retrieve event information
				var targetEvent = await events.GetEventInformationAsync(user.Id, eventId);

				return Ok(targetEvent);
			});
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDetailsManifest eventDetails)
        {
			// Verify parameters
            if (eventDetails == null || !ModelState.IsValid)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				// Create a new event
				var newEvent = await events.CreateEventAsync(user.Id,
					eventDetails.EventName, eventDetails.EventDescription,
					eventDetails.StartTime, eventDetails.Latitude, eventDetails.Longitude,
					eventDetails.Radius, eventDetails.IsDynamic,
					eventDetails.GroupMinimum, eventDetails.GroupMaximum);

				return Ok(newEvent);
			});
        }

        [HttpPost("{eventId}/edit")]
        public async Task<IActionResult> EditEvent(ulong eventId, [FromBody] EventDetailsManifest eventDetails)
		{
			// Verify parameters
			if (eventDetails == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await events.EditEventAsync(user.Id, eventId,
					eventDescription: eventDetails.EventDescription ?? "",
					isOpen: eventDetails.IsOpen,
					startTime: eventDetails.StartTime,
					latitude: eventDetails.Latitude, longitude: eventDetails.Longitude,
					radius: eventDetails.Radius, isDynamic: eventDetails.IsDynamic,
					groupMinimum: eventDetails.GroupMinimum, groupMaximum: eventDetails.GroupMaximum);
			});
		}

		[HttpGet("{eventId}/start")]
		public async Task<IActionResult> StartEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// Start event
				await events.StartEventAsync(user.Id, eventId);
			});
		}

        [HttpDelete("{eventId}/edit")]
        public async Task<IActionResult> EndEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// End an event
				await events.EndEventAsync(user.Id, eventId);
			});
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// Delete an event
				await events.DeleteEventAsync(user.Id, eventId);
			});
        }

		[HttpPost("{eventId}/watch")]
		public async Task<IActionResult> WatchEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// Join an event
				await events.WatchEventAsync(user.Id, eventId);
			});
		}

		[HttpPut("{eventId}/watch")]
		public async Task<IActionResult> UnwatchEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// Join an event
				await events.UnwatchEventAsync(user.Id, eventId);
			});
		}

		[HttpPost("{eventId}")]
        public async Task<IActionResult> JoinEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// Join an event
				await events.JoinEventAsync(user.Id, eventId);
			});
		}

		[HttpPut("{eventId}")]
		public async Task<IActionResult> LeaveEvent(ulong eventId)
		{
			return await Execute(async user =>
			{
				// Leave an event
				await events.LeaveEventAsync(user.Id, eventId);
			});
		}

		[HttpGet("{eventId}/guests")]
		public async Task<IActionResult> GetGuestList(ulong eventId)
		{
			return await Execute(async user =>
			{
				var guestList = await events.GetGuestListAsync(user.Id, eventId);

				return Ok(guestList);
			});
		}

		[HttpGet("{eventId}/invite")]
		public async Task<IActionResult> GetPotentialInvitees(ulong eventId)
		{
			return await Execute(async user =>
			{
				var users = await events.GetPotentialInviteesAsync(user.Id, eventId);

				return Ok(users);
			});
        }

		[HttpPost("{eventId}/invite")]
		public async Task<IActionResult> InviteUser(ulong inviteeId, ulong eventId)
		{
			return await Execute(async user =>
			{
				await events.InviteUserAsync(user.Id, inviteeId, eventId);
			});
        }

		[HttpPut("{eventId}/guests")]
		public async Task<IActionResult> KickUser(ulong targetId, ulong eventId)
		{
			return await Execute(async user =>
			{
				await events.KickUserAsync(user.Id, targetId, eventId);
			});
		}

		[HttpPost("{eventId}/report")]
		public async Task<IActionResult> ReportEvent(ulong eventId, [FromBody] EventReportManifest report)
		{
			// Verify parameters
			if (report == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await reports.ReportEventAsync(user.Id, eventId, report.ReportType, report.ReportDetails);
			});
		}

		[HttpGet("{eventId}/etchings")]
		public async Task<IActionResult> GetEventEtchings(ulong eventId)
		{
			return await Execute(async user =>
			{
				var eventEtchings = await etchings.GetEventEtchingsAsync(user.Id, eventId);

				return Ok(eventEtchings);
			});
		}

		[HttpPost("{eventId}/etchings")]
		public async Task<IActionResult> EtchingToEvent(ulong eventId)
		{
			// Verify parameters
			if (!ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				var newEtching = await etchings.AddEtchingAsync(user.Id, eventId, await StreamFirstFile());

				return Ok(newEtching);
			});
		}

		[HttpPut("{eventId}/etchings")]
		public async Task<IActionResult> RemoveEtching(ulong eventId, ulong etchingId)
		{
			return await Execute(async user =>
			{
				await etchings.RemoveEtchingAsync(user.Id, etchingId);
			});
		}

		[HttpPost("{eventId}/etchings/{etchingId}")]
		public async Task<IActionResult> RateEtching(ulong eventId, ulong etchingId, [FromBody] AccountRatingManifest details)
		{
			// Verify parameters
			if (details == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await etchings.RateEtchingAsync(user.Id, etchingId, details.Rating);
			});
		}

		#endregion
	}
}