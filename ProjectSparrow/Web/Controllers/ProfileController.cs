using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Models;
using Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
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

		IAccountOperations accounts;
		UserManager<ThinUser> userManager;

		public ProfileController(IAccountOperations accountOperations, UserManager<ThinUser> identityUserManager)
		{
			accounts = accountOperations;
			userManager = identityUserManager;
		}

		[HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetUser(string targetIdentification)
        {
			if (targetIdentification == null)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			ThinProfile profile;

			try
			{
				// Parse target identification and retrieve profile
				var target = GetGUID(targetIdentification);
				var user = await GetCurrentUserAsync();
				profile = await accounts.GetUserProfileAsync(user.Id, target);
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

		[HttpGet("{targetIdentification}/activity")]
		public async Task<IActionResult> GetUserActivity(string targetIdentification)
		{
			if (targetIdentification == null)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			List<ThinEvent> activity;

			try
			{
				// Parse target identification and retrieve activity
				var target = GetGUID(targetIdentification);
				var user = await GetCurrentUserAsync();
				activity = await accounts.GetUserActivityAsync(user.Id, target);
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

		[HttpGet("following")]
        public async Task<IActionResult> GetFollowed()
        {
			List<ThinnerUser> followedUsers;

			try
			{
				// Retrieve all users that the current user is following
				var user = await GetCurrentUserAsync();
				followedUsers = await accounts.GetFollowedUsersAsync(user.Id);
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
				await accounts.FollowUserAsync(user.Id, info.TargetID);
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
				await accounts.UnfollowUserAsync(user.Id, info.TargetID);
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
			List<ThinnerUser> blockedUsers; // Change to something more meaningful

			try
			{
				// Retrieve all users that the current user is blocking
				var user = await GetCurrentUserAsync();
				blockedUsers = await accounts.GetBlockedUsersAsync(user.Id);
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
				await accounts.BlockUserAsync(user.Id, info.TargetID);
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
				await accounts.UnblockUserAsync(user.Id, info.TargetID);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
		}

		private async Task<ThinUser> GetCurrentUserAsync()
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
