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

		public FeedGuard(ILogger logger,
			UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, IKeyOperations keyOperations,
			IMediaOperations mediaOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(logger,
				identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				disciplineOperations, keyOperations,
				mediaOperations, notificationOperations,
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
				var shard = await etchings.GetUserFeedAsync(user.Id, depth, lastDepth);

				FeedManifest feed = new()
				{
					Headers = shard.Headers.ConvertAll(header => new EventHeaderManifest(header)),
					Etchings = shard.Etchings.ConvertAll(etching => new EtchingManifest(etching))
				};

				return feed;
			});
        }

		#endregion
	}
}