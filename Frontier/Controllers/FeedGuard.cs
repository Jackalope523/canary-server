using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("feed")]
    public class FeedGuard : AbstractGuard
    {
		#region Initialisation

		public FeedGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet("{depth}-{lastDepth}")]
        public async Task<IActionResult> GetFeed(int depth, int lastDepth)
        {
			// Verify parameters
            if (!ModelState.IsValid)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await snapshots.GetUserFeedAsync(user.Id, depth, lastDepth));
        }

		#endregion
	}
}