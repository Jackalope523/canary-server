using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Controllers
{
    [Route("discover")]
    [ApiController]
    [Authorize]
    public class DiscoverAgent : ControllerBase
    {
        enum DiscoverError
        {
            MissingInformation,
            CouldNotFindEvent,
            CouldNotCompleteRequest
        }

        IAccountOperations accounts;
        IEventOperations events;
		UserManager<UserShard> userManager;

		public DiscoverAgent(IAccountOperations accountOperations, IEventOperations eventOperations, UserManager<UserShard> identityUserManager)
        {
            accounts = accountOperations;
            events = eventOperations;
            userManager = identityUserManager;
        }

        [HttpGet("{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetEvents(float latitude, float longitude, float distance)
        {
            List<EventThinSlice> eventList;

            try
            {
                // Retrieve events personalised for the current user
                var user = await GetCurrentUserAsync();
                eventList = await events.GetPersonalisedEventsInAreaAsync(user.Id, latitude, longitude, distance);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventList);
        }

        [HttpGet("all/{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetAllEvents(float latitude, float longitude, float distance)
        {
            List<EventThinSlice> eventList;

            try
			{
                // Retrieve all events available to the current user
				var user = await GetCurrentUserAsync();
				eventList = await events.GetEventsInAreaAsync(user.Id, latitude, longitude, distance);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventList);
        }

        [HttpPost("user/{latitude}-{longitude}")]
        public async Task<IActionResult> UpdateCurrentPosition(float latitude, float longitude)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                await accounts.UpdateUserLocationAsync(user.Id, latitude, longitude);
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
