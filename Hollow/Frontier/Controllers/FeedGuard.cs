using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;

namespace Frontier.Controllers
{
    [Route("feed")]
    public class FeedGuard : AbstractGuard
    {
		#region Initialisation

		public FeedGuard(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				disciplineOperations, notificationOperations,
				externalSMSService, externalEmailService)
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
			{
				var userFeed = await etchings.GetUserFeedAsync(user.Id, depth, lastDepth);

				return Ok(userFeed);
			});
        }

		#endregion
	}
}