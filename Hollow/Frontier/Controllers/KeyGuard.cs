using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
	[Route("keys")]
	public class KeyGuard : AbstractGuard
	{
		#region Initialisation

		public KeyGuard(ILogger logger,
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

		[HttpGet("{key}")]
		public async Task<IActionResult> GetSecret(string key)
		{
			return await Execute(async user =>
			{
				var secret = await keys.GetSecretAsync(user.Id, key);

				return Ok(secret);
			});
        }

		#endregion
	}
}