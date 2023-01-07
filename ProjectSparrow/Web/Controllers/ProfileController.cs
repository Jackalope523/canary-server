using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    [Route("accounts/[controller]")]
    [ApiController]
    public class ProfileController : Controller
	{
		enum ProfileError
		{
			MissingInformation,
			CouldNotCompleteRequest,
			CouldNotFindUser
		}

		private IAccountOperations accounts;

		public ProfileController(IAccountOperations accountOperations)
		{
			accounts = accountOperations;
		}

		[HttpGet("{targetIdentification}")]
        public IActionResult GetUser([FromBody] ProfileModel info)
        {
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			ThinProfile profile;

			try
			{
				profile = accounts.GetUserProfile(info.Identification, info.TargetIdentification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

            return Ok(profile);
        }

        [HttpGet("following")]
        public IActionResult GetFollowed([FromBody] IdentifierModel user)
        {
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			List<ThinListUser> followedUsers; // Change to something more meaningful

			try
			{
				followedUsers = accounts.GetFollowedUsers(user.Identification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

			return Ok(followedUsers);
		}

		[HttpPost("following")]
		public IActionResult FollowUser([FromBody] ProfileModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				accounts.FollowUser(info.Identification, info.TargetIdentification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

		[HttpPut("following")]
		public IActionResult UnfollowUser([FromBody] ProfileModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				accounts.UnfollowUser(info.Identification, info.TargetIdentification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

		[HttpGet("blocked")]
		public IActionResult GetBlocked([FromBody] IdentifierModel user)
		{
			if (user == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			List<ThinListUser> blockedUsers; // Change to something more meaningful

			try
			{
				blockedUsers = accounts.GetBlockedUsers(user.Identification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

			return Ok(blockedUsers);
		}

		[HttpPost("blocked")]
		public IActionResult BlockUser([FromBody] ProfileModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				accounts.BlockUser(info.Identification, info.TargetIdentification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}

		[HttpPut("blocked")]
		public IActionResult UnblockUser([FromBody] ProfileModel info)
		{
			if (info == null || !ModelState.IsValid)
			{
				return BadRequest(ProfileError.MissingInformation.ToString());
			}

			try
			{
				accounts.UnblockUser(info.Identification, info.TargetIdentification);
			}
			catch
			{
				return BadRequest(ProfileError.CouldNotCompleteRequest.ToString());
			}

			return Ok();
		}
	}

}
