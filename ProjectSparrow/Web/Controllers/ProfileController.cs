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
    [Route("profiles/[controller]")]
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
				var user = await GetCurrentUserAsync();

				if (!Guid.TryParse(targetIdentification, out Guid targetGuid))
				{
					throw new ArgumentException("Not a valid GUID.", nameof(targetIdentification));
				}

				profile = await accounts.GetUserProfileAsync(user.Id, targetGuid);
			}
			catch (InvalidUserException e)
			{
				return BadRequest(e.ToString());
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(profile);
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetFollowed()
        {
			List<ThinnerUser> followedUsers; // Change to something more meaningful

			try
			{
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
	}

}
