using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Core.Boundaries;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontier.Controllers
{
	[ApiController]
	[Authorize]
	public class AbstractGuard : ControllerBase
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
		public IEventOperations events;
		public IEtchingOperations etchings;
		public IDisciplineOperations reports;
		public IMediaOperations media;
		public INotificationOperations notifications;
		public IProfileOperations profiles;

		public ISMSService smsService;
		public IEmailService emailService;

		#endregion

		#region Initialisation

		public AbstractGuard(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, IMediaOperations mediaOperations,
			INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService)
		{
			userManager = identityUserManager;
			signInManager = identitySignInManager;

			accounts = accountOperations;
			profiles = profileOperations;
			events = eventOperations;
			etchings = etchingOperations;
			reports = disciplineOperations;
			media = mediaOperations;
			notifications = notificationOperations;

			smsService = externalSMSService;
			emailService = externalEmailService;
		}

		#endregion

		#region Favours

		[NonAction]
		public async Task<IActionResult> Execute(Func<Task<IActionResult>> action)
		{
			try
			{
				return await action.Invoke();
			}
			catch (HollowFailureException ex)
			{
				// Log failure
				Log.Error(ex, ex.Message);

				return StatusCode(500, ex.StackTrace);
			}
			catch (UserErrorException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				// Log failure
				Log.Error(ex, ex.Message);

				return StatusCode(500, ex.StackTrace);
			}
		}

		[NonAction]
		public async Task<IActionResult> Execute(Func<Task> action)
		{
			return await Execute(async () =>
			{
				await action.Invoke();
				return Ok();
			});
		}

		[NonAction]
		public async Task<IActionResult> Execute(Func<UserShard, Task> action, bool allowUnverified = false)
		{
			return await Execute(async user =>
			{
				await action.Invoke(user);
				return Ok();
			},
			allowUnverified);
		}

		[NonAction]
		public async Task<IActionResult> Execute(Func<UserShard, Task<IActionResult>> action, bool allowUnverified = false)
		{
			return await Execute(async () =>
			{
				var user = await GetCurrentUserAsync();

				if (!allowUnverified)
				{ ThrowIfUnverified(user); }

				return await action.Invoke(user);
			});
		}

		[NonAction]
		public async Task<UserShard> GetCurrentUserAsync()
			=> await userManager.GetUserAsync(HttpContext.User);

		[NonAction]
		public void ThrowIfUnverified(UserShard user)
		{
			if (user.IsEmailConfirmed)
			{ throw new InvalidUserException("User has not yet confirmed their email."); }
		}

		[NonAction]
		public async Task<MemoryStream> StreamFirstFile()
		{
			foreach (var file in Request.Form.Files)
			{
				if (file.Length > 0)
				{
					using var ms = new MemoryStream();
					await file.CopyToAsync(ms);
					return ms;
				}
			}

			return null;
		}

		#endregion
	}
}