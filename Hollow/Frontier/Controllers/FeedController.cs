using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Boundaries;
using Frontier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Controllers
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

        IEtchingOperations etchings;
        UserManager<UserShard> userManager;

        public FeedController(IEtchingOperations etchingOperations, UserManager<UserShard> identityUserManager)
        {
            etchings = etchingOperations;
            userManager = identityUserManager;
        }

        [HttpGet("{feedDepth}")]
        public async Task<IActionResult> GetFeed([FromBody] FeedModel feedOptions)
        {
            if (feedOptions == null || !ModelState.IsValid)
            {
                return BadRequest(FeedError.MissingInformation.ToString());
            }

            (int Depth, List<EventHeader>, List<Etching>) userFeed;

            try
            {
                // Retrieve current user
                var user = await GetCurrentUserAsync();
                userFeed = await etchings.GetUserFeedAsync(user.Id, feedOptions.Depth, feedOptions.ExclusionList.ToList());
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
