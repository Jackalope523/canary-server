using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Core.Boundaries;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Frontier.Manifests;
using System.Collections.Generic;

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
			CouldNotFindGathering,
		}

		#endregion

		#region Variables

		public ILogger log;

		public IAccountOperations accounts;
		public IBannerOperations banners;
		public IGatheringOperations gatherings;
		public ISnapshotOperations snapshots;
		public IDisciplineOperations reports;
		public IKeyOperations keys;
		public IMediaOperations media;
		public INotificationOperations notifications;
		public INestOperations nests;

		public UserManager<CoreUser> userManager;

		#endregion

		#region Initialisation

		public AbstractGuard(GuardBox box, UserManager<CoreUser> aspUserManager)
		{
			log = box.log;

			accounts = box.accounts;
			banners = box.banners;
			nests = box.nests;
			gatherings = box.gatherings;
			snapshots = box.snapshots;
			keys = box.keys;
			reports = box.reports;
			media = box.media;
			notifications = box.notifications;

			userManager = aspUserManager;
		}

		#endregion

		#region Favours

		[NonAction]
		public async Task<IActionResult> Execute(Func<Task<object>> action)
		{
			try
			{
				var result = await action.Invoke();

                // Check if there is a result
                if (result == null)
                {
					Ok();
                }

                // Ensure outgoing type is generic or manifest
                if (result is CoreOnlyData)
                { throw new UnexpectedFailureException($"Server tried sending Core-Only object {result.GetType()}."); }

                return Ok(result);
			}
			catch (HollowFailureException ex)
			{
				// Log failure
				log.LogError("\nHollow Exception\n{message}\n{trace}", ex.Message, ex.StackTrace);

				return StatusCode(500);
			}
			catch (UserErrorException ex)
			{
				// Log debug information
				log.LogDebug("\nUser Exception\n{message}\n{trace}", ex.Message, ex.StackTrace);

                return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				// Log failure
				log.LogError("\nHollow Exception\n{message}\n{trace}", ex.Message, ex.StackTrace);

				return StatusCode(500);
			}
		}

		[NonAction]
		public async Task<IActionResult> Execute(Func<Task> action)
		{
			return await Execute(async () =>
			{
				await action.Invoke();
				return null;
			});
		}

		[NonAction]
		public async Task<IActionResult> Execute(Func<CoreUser, Task> action, bool allowUnverified = false)
		{
			return await Execute(async user =>
			{
				await action.Invoke(user);
				return null;
			},
			allowUnverified);
		}

		[NonAction]
		public async Task<IActionResult> Execute(Func<CoreUser, Task<object>> action, bool allowUnverified = false)
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
		public async Task<CoreUser> GetCurrentUserAsync()
			=> await userManager.GetUserAsync(HttpContext.User);

		[NonAction]
		public void ThrowIfUnverified(CoreUser user)
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