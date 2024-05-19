using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;

using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("profile")]
    public class ProfileGuard : AbstractGuard
	{
		#region Initialisation

		public ProfileGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetProfile(ulong targetIdentification)
		{
			return await Execute(async user =>
				await profiles.GetUserProfileAsync(user.Id, targetIdentification));
		}

		[HttpGet("{targetIdentification}/nest")]
        public async Task<IActionResult> GetNest(ulong targetIdentification)
		{
			return await Execute(async user =>
				await profiles.GetUserNestAsync(user.Id, targetIdentification));
		}

		[HttpGet("companions")]
        public async Task<IActionResult> GetCompanions()
        {
            return await Execute(async user =>
                await profiles.GetCompanionsAsync(user.Id));
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetFollowed()
		{
			return await Execute(async user =>
				await profiles.GetFollowedUsersAsync(user.Id));
		}

		[HttpPost("following")]
		public async Task<IActionResult> FollowUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await profiles.FollowUserAsync(user.Id, info.TargetId));
		}

		[HttpPut("following")]
		public async Task<IActionResult> UnfollowUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await profiles.UnfollowUserAsync(user.Id, info.TargetId));
		}

		[HttpGet("blocked")]
		public async Task<IActionResult> GetBlocked()
		{
			return await Execute(async user =>
				await profiles.GetBlockedUsersAsync(user.Id));
		}

		[HttpPost("blocked")]
		public async Task<IActionResult> BlockUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await profiles.BlockUserAsync(user.Id, info.TargetId));
		}

		[HttpPut("blocked")]
		public async Task<IActionResult> UnblockUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await profiles.UnblockUserAsync(user.Id, info.TargetId));
		}

		[HttpPost("{targetIdentification}/report")]
		public async Task<IActionResult> ReportUser(ulong targetId, [FromBody] AccountReportManifest report)
		{
			// Verify parameters
			if (report == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await reports.ReportUserAsync(user.Id, targetId, report.ReportType, report.ReportDetails));
		}

		#endregion
	}
}