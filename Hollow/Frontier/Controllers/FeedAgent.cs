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
    public class FeedAgent : AbstractAgent
    {
		#region Initialisation

		public FeedAgent(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
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

		[HttpGet("{feedDepth}")]
        public async Task<IActionResult> GetFeed([FromBody] FeedManifest feedOptions)
        {
			// Verify parameters
            if (feedOptions == null || !ModelState.IsValid)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				var userFeed = await etchings.GetUserFeedAsync(user.Id, feedOptions.Depth, feedOptions.ExclusionList.ToList());

				return Ok(userFeed);
			});
        }

		#endregion
	}
}