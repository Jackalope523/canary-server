using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DiscoverController : Controller
    {
        enum DiscoverError
        {
            MissingInformation,
            CouldNotFindEvent,
            CouldNotCompleteRequest
        }

        IEventOperations events;
		UserManager<ThinUser> userManager;

		public DiscoverController(IEventOperations eventOperations, UserManager<ThinUser> identityUserManager)
        {
            events = eventOperations;
            userManager = identityUserManager;
        }

        [HttpGet("{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetEvents(float latitude, float longitude, float distance)
        {
            List<ThinnerEvent> eventList;

            try
            {
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
            List<ThinnerEvent> eventList;

            try
			{
				var user = await GetCurrentUserAsync();

				eventList = await events.GetEventsInAreaAsync(user.Id, latitude, longitude, distance);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventList);
        }

		private async Task<ThinUser> GetCurrentUserAsync()
        {
			return await userManager.GetUserAsync(HttpContext.User);
		}
	}

}
