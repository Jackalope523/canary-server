using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("error")]
    public class ErrorGuard : AbstractGuard
    {
		#region Initialisation

		public ErrorGuard(ILogger logger,
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

		[HttpGet]
        public IActionResult Error()
		{
			return new StatusCodeResult(418);
		}

		#endregion
	}
}