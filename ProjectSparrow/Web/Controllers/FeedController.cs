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
    [Route("feed")]
    [ApiController]
    [Authorize]
    public class FeedController : Controller
    {
        enum FeedError
        {
            MissingInformation,
            CouldNotFindEvent,
            CouldNotCompleteRequest
        }

        IAccountOperations accounts;
        IEventOperations events;
		UserManager<ThinUser> userManager;

		public FeedController(IAccountOperations accountOperations, IEventOperations eventOperations, UserManager<ThinUser> identityUserManager)
        {
            accounts = accountOperations;
            events = eventOperations;
            userManager = identityUserManager;
        }

        [HttpGet("{feedDepth}")]
        public async Task<IActionResult> GetFeed(int feedDepth)
        {
            if (feedDepth < 0)
            {
                return BadRequest(FeedError.MissingInformation.ToString());
            }

            (int Depth, List<EventPost>) userFeed;

            try
            {
                // Retrieve current user
                var user = await GetCurrentUserAsync();
                userFeed = await events.GetUserFeedAsync(user.Id, feedDepth);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(userFeed);
        }

		private async Task<ThinUser> GetCurrentUserAsync()
        {
			return await userManager.GetUserAsync(HttpContext.User);
		}
	}

}
