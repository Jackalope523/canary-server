using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Frontier.Models;
using Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Frontier.Controllers
{
    [Route("profile")]
    [ApiController]
	[Authorize]
    public class ProfileController : Controller
	{
		enum ProfileError
		{
			MissingInformation,
			CouldNotCompleteRequest,
			CouldNotFindUser
		}

		IProfileOperations profiles;
		IReportOperations reports;
		UserManager<UserShard> userManager;

		public ProfileController(IProfileOperations profileOperations,
			IReportOperations reportOperations,UserManager<UserShard> identityUserManager)
		{
			profiles = profileOperations;
			reports = reportOperations;
			userManager = identityUserManager;
		}

		[HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetUser(string targetIdentification)
        {
			if (targetIdentification == null)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			UserProfile profile;

			try
			{
				// Parse target identification and retrieve profile
				var target = GetGUID(targetIdentification);
				var user = await GetCurrentUserAsync();
				profile = await profiles.GetUserProfileAsync(user.Id, target);
			}
			catch (InvalidUserException e)
			{
				// User does not exist
				return BadRequest(e.ToString());
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(profile);
		}

		[HttpPost("{targetIdentification}")]
		public async Task<IActionResult> RateUser(string targetIdentification, [FromBody] AccountRatingModel details)
        {
            if (targetIdentification == null && details != null && !ModelState.IsValid)
            {
                return BadRequest(ProfileError.MissingInformation.ToString());
            }

			try
			{
				var target = GetGUID(targetIdentification);
				var user = await GetCurrentUserAsync();
				await profiles.RateUserAsync(user.Id, target, details.Rating);
            }
            catch (InvalidUserException e)
            {
                // User does not exist
                return BadRequest(e.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Ok();
		}

		[HttpGet("{targetIdentification}/activity")]
		public async Task<IActionResult> GetUserActivity(string targetIdentification)
		{
			if (targetIdentification == null)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			List<EventShard> activity;

			try
			{
				// Parse target identification and retrieve activity
				var target = GetGUID(targetIdentification);
				var user = await GetCurrentUserAsync();
				activity = await profiles.GetUserActivityAsync(user.Id, target);
			}
			catch (InvalidUserException e)
			{
				// User does not exist
				return BadRequest(e.ToString());
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(activity);
		}

		[HttpGet("activity")]
		public async Task<IActionResult> GetFriendActivity()
		{
			Dictionary<UserSilhouette, List<EventShard>> activity;

			try
			{
				// Retrieve activity
				var user = await GetCurrentUserAsync();
				activity = await profiles.GetFriendActivityAsync(user.Id);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(activity);
		}

		[HttpGet("following")]
        public async Task<IActionResult> GetFollowed()
        {
			List<UserSilhouette> followedUsers;

			try
			{
				// Retrieve all users that the current user is following
				var user = await GetCurrentUserAsync();
				followedUsers = await profiles.GetFollowedUsersAsync(user.Id);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(followedUsers);
		}

		[HttpPost("following")]
		public async Task<IActionResult> FollowUser([FromBody] TargetModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Follow other user
				var user = await GetCurrentUserAsync();
				await profiles.FollowUserAsync(user.Id, info.TargetID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("following")]
		public async Task<IActionResult> UnfollowUser([FromBody] TargetModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Unfollow other user
				var user = await GetCurrentUserAsync();
				await profiles.UnfollowUserAsync(user.Id, info.TargetID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpGet("blocked")]
		public async Task<IActionResult> GetBlocked()
		{
			List<UserSilhouette> blockedUsers; // Change to something more meaningful

			try
			{
				// Retrieve all users that the current user is blocking
				var user = await GetCurrentUserAsync();
				blockedUsers = await profiles.GetBlockedUsersAsync(user.Id);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(blockedUsers);
		}

		[HttpPost("blocked")]
		public async Task<IActionResult> BlockUser([FromBody] TargetModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Block other user
				var user = await GetCurrentUserAsync();
				await profiles.BlockUserAsync(user.Id, info.TargetID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("blocked")]
		public async Task<IActionResult> UnblockUser([FromBody] TargetModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Unblock other user
				var user = await GetCurrentUserAsync();
				await profiles.UnblockUserAsync(user.Id, info.TargetID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{targetIdentification}/report")]
		public async Task<IActionResult> ReportUser(string targetID, [FromBody] AccountReportModel report)
		{
			if (string.IsNullOrEmpty(targetID) || report == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				Guid targetGUID = GetGUID(targetID);
				await reports.ReportUserAsync(user.Id, targetGUID, report.ReportType, report.ReportDetails);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		private async Task<UserShard> GetCurrentUserAsync()
		{
			return await userManager.GetUserAsync(HttpContext.User);
		}

		private Guid GetGUID(string id)
		{
			if (!Guid.TryParse(id, out Guid guid))
			{
				throw new ArgumentException("Not a valid GUID.", nameof(id));
			}

			return guid;
		}
	}

}
