using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Server.Boundaries;
using Web.Models;
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
		UserManager<UserShard> userManager;

		public FeedController(IAccountOperations accountOperations, IEventOperations eventOperations, UserManager<UserShard> identityUserManager)
        {
            accounts = accountOperations;
            events = eventOperations;
            userManager = identityUserManager;
        }

        [HttpGet("{feedDepth}")]
        public async Task<IActionResult> GetFeed([FromBody] FeedModel feedOptions)
        {
            if (feedOptions == null || !ModelState.IsValid)
            {
                return BadRequest(FeedError.MissingInformation.ToString());
            }

            (int Depth, List<EventHeader>, List<EventPost>) userFeed;

            try
            {
                // Retrieve current user
                var user = await GetCurrentUserAsync();
                userFeed = await events.GetUserFeedAsync(user.Id, feedOptions.Depth, feedOptions.ExclusionList.ToList());
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(userFeed);
        }

		private async Task<UserShard> GetCurrentUserAsync()
        {
			return await userManager.GetUserAsync(HttpContext.User);
		}
	}

}
