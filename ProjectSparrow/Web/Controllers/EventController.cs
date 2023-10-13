using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Web.Models;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Twilio.TwiML.Voice;
using Microsoft.Extensions.Hosting;
using Shared;

namespace Web.Controllers
{
    [Route("event")]
    [ApiController]
	[Authorize]
    public class EventController : ControllerBase
    {
        enum EventError
		{
			MissingInformation,
			CouldNotFindEvent,
			CouldNotCompleteRequest
		}

		IEventOperations events;
		IPostOperations posts;
		IReportOperations reports;
		UserManager<UserShard> userManager;

		public EventController(IEventOperations eventOperations,
			IPostOperations postOperations, IReportOperations reportOperations,
			UserManager<UserShard> identityUserManager)
        {
            events = eventOperations;
			posts = postOperations;
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
        public async Task<IActionResult> CreateEvent([FromBody] EventDetailsModel eventDetails)
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
        public async Task<IActionResult> EditEvent(string eventID, [FromBody] EventEditModel eventDetails)
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
		public async Task<IActionResult> ReportEvent(string eventID, string hostID, [FromBody] EventReportModel report)
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

		[HttpGet("{eventID}/posts")]
		public async Task<IActionResult> GetEventPosts(string eventID)
		{
			if (string.IsNullOrEmpty(eventID))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			List<EventPost> eventPosts;

			try
			{
				// Retrieve event information as current user
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);

				eventPosts = await posts.GetEventPostsAsync(user.Id, eventGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventPosts);
		}

		[HttpPost("{eventID}/posts")]
		public async Task<IActionResult> PostToEvent(string eventID, [FromBody] EventPostModel post)
		{
			if (string.IsNullOrEmpty(eventID) || post == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			EventPost newPost;

			try
			{
				var user = await GetCurrentUserAsync();
				Guid eventGUID = GetGUID(eventID);
				newPost = await posts.AddPostAsync(user.Id, eventGUID, post.ImageURL);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(newPost);
		}

		[HttpPut("{eventID}/posts")]
		public async Task<IActionResult> RemovePost(string eventID, string postID)
		{
			if (string.IsNullOrEmpty(eventID) || string.IsNullOrEmpty(postID))
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				Guid postGUID = GetGUID(postID);
				await posts.RemovePostAsync(user.Id, postGUID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{eventID}/posts/{postID}")]
		public async Task<IActionResult> RatePost(string eventID, string postID, [FromBody] AccountRatingModel details)
		{
			if (string.IsNullOrEmpty(eventID) || details == null || !ModelState.IsValid)
			{
				return BadRequest(EventError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				var postGUID = GetGUID(postID);
				await posts.RatePostAsync(user.Id, postGUID, details.Rating);
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
