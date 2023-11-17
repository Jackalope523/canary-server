using Core.Boundaries;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Frontier.Controllers
{
	[ApiController]
	[Authorize]
	public class AbstractAgent : ControllerBase
	{
		#region Schemas

		public enum HollowError
		{
			MissingInformation,
			CouldNotCompleteRequest,
			IncorrectCode,
			UserLockedOut,
			CouldNotLoginUser,
			CouldNotCreateUser,
			CouldNotModifyUser,
			CouldNotFindUser,
			CouldNotFindEvent,
		}

		#endregion

		#region Variables

		public UserManager<UserShard> userManager;
		public SignInManager<UserShard> signInManager;

		public IAccountOperations accounts;
		public IProfileOperations profiles;
		public IEventOperations events;
		public IEtchingOperations etchings;
		public IReportOperations reports;
		public INotificationOperations notifications;

		public ISMSService smsService;
		public IEmailService emailService;

		#endregion

		#region Initialisation

		public AbstractAgent(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IReportOperations reportOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService)
		{
			userManager = identityUserManager;
			signInManager = identitySignInManager;

			accounts = accountOperations;
			profiles = profileOperations;
			events = eventOperations;
			etchings = etchingOperations;
			reports = reportOperations;
			notifications = notificationOperations;

			smsService = externalSMSService;
			emailService = externalEmailService;
		}

		#endregion

		#region Favours

		public async Task<UserShard> GetCurrentUserAsync()
		{
			return await userManager.GetUserAsync(HttpContext.User);
		}

		#endregion
	}
}