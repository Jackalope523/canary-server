using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Frontier.Manifests;
using Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Frontier.Controllers
{
    [Route("profile")]
    [ApiController]
	[Authorize]
    public class ProfileAgent : ControllerBase
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

		public ProfileAgent(IProfileOperations profileOperations,
			IReportOperations reportOperations,UserManager<UserShard> identityUserManager)
		{
			profiles = profileOperations;
			reports = reportOperations;
			userManager = identityUserManager;
		}

		[HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetProfile(ulong targetIdentification)
        {
			UserProfile profile;

			try
			{
				// Retrieve profile
				var user = await GetCurrentUserAsync();
				profile = await profiles.GetUserProfileAsync(user.Id, targetIdentification);
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

		[HttpGet("{targetIdentification}/nest")]
        public async Task<IActionResult> GetNest(ulong targetIdentification)
        {
			(List<EventThinSlice> Events, List<Etching> Etchings) nest;

			try
			{
				// Retrieve nest
				var user = await GetCurrentUserAsync();
				nest = await profiles.GetUserNestAsync(user.Id, targetIdentification);
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

			return Ok(nest);
		}

		[HttpPost("{targetIdentification}")]
		public async Task<IActionResult> RateUser(ulong targetIdentification, [FromBody] AccountRatingManifest details)
        {
            if (details != null && !ModelState.IsValid)
            {
                return BadRequest(ProfileError.MissingInformation.ToString());
            }

			try
			{
				var user = await GetCurrentUserAsync();
				await profiles.RateUserAsync(user.Id, targetIdentification, details.Rating);
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
		public async Task<IActionResult> GetUserActivity(ulong targetIdentification)
		{
			List<EventShard> activity;

			try
			{
				// Retrieve activity
				var user = await GetCurrentUserAsync();
				activity = await profiles.GetUserActivityAsync(user.Id, targetIdentification);
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
		public async Task<IActionResult> FollowUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Follow other user
				var user = await GetCurrentUserAsync();
				await profiles.FollowUserAsync(user.Id, info.TargetId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("following")]
		public async Task<IActionResult> UnfollowUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Unfollow other user
				var user = await GetCurrentUserAsync();
				await profiles.UnfollowUserAsync(user.Id, info.TargetId);
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
		public async Task<IActionResult> BlockUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Block other user
				var user = await GetCurrentUserAsync();
				await profiles.BlockUserAsync(user.Id, info.TargetId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPut("blocked")]
		public async Task<IActionResult> UnblockUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				// Unblock other user
				var user = await GetCurrentUserAsync();
				await profiles.UnblockUserAsync(user.Id, info.TargetId);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		[HttpPost("{targetIdentification}/report")]
		public async Task<IActionResult> ReportUser(ulong targetId, [FromBody] AccountReportManifest report)
		{
			if (report == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				var user = await GetCurrentUserAsync();
				await reports.ReportUserAsync(user.Id, targetId, report.ReportType, report.ReportDetails);
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
	}

}
