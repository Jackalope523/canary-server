using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;

namespace Frontier.Controllers
{
    [Route("error")]
    public class ErrorAgent : AbstractAgent
    {
		#region Initialisation

		public ErrorAgent(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
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

		[HttpGet]
        public IActionResult Error()
		{
			return new StatusCodeResult(418);
		}

		#endregion
	}
}