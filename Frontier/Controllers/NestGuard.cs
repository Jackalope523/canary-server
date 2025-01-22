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
    [Route("nest")]
    public class NestGuard : AbstractGuard
	{
		#region Initialisation

		public NestGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet("{targetId}")]
        public async Task<IActionResult> GetNest(long targetId)
		{
			return await Execute(async user =>
				await nests.GetNestAsync(user.Id, targetId));
		}

		[HttpGet("companions")]
        public async Task<IActionResult> GetCompanions()
        {
            return await Execute(async user =>
                await nests.GetCompanionsAsync(user.Id));
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetFollowed()
		{
			return await Execute(async user =>
				await nests.GetFollowedUsersAsync(user.Id));
		}

		[HttpPost("following")]
		public async Task<IActionResult> FollowUser(long? targetId = null, string code = null)
		{
			// Verify parameters
			if (!ModelState.IsValid)
			{ return MissingInformation(); }

			return await Execute(async user =>
			{
				if (targetId.HasValue)
				{ await nests.FollowUserAsync(user.Id, targetId.Value); }
				else if (!string.IsNullOrEmpty(code))
				{ await nests.FollowUserAsync(user.Id, code); }
				else
				{ throw new MissingInformationException(); }
			});
		}

		[HttpPut("following")]
		public async Task<IActionResult> UnfollowUser(long targetId)
		{
			// Verify parameters
			if (!ModelState.IsValid)
			{ return MissingInformation(); }

			return await Execute(async user =>
				await nests.UnfollowUserAsync(user.Id, targetId));
		}

		[HttpGet("blocked")]
		public async Task<IActionResult> GetBlocked()
		{
			return await Execute(async user =>
				await nests.GetBlockedUsersAsync(user.Id));
		}

		[HttpPost("blocked")]
		public async Task<IActionResult> BlockUser(long targetId)
		{
			// Verify parameters
			if (!ModelState.IsValid)
			{ return MissingInformation(); }

			return await Execute(async user =>
				await nests.BlockUserAsync(user.Id, targetId));
		}

		[HttpPut("blocked")]
		public async Task<IActionResult> UnblockUser(long targetId)
		{
			if (!ModelState.IsValid)
			{ return MissingInformation(); }

			return await Execute(async user =>
				await nests.UnblockUserAsync(user.Id, targetId));
		}

		[HttpGet("{targetId}/authorisation/follow")]
		public async Task<IActionResult> CheckFollowAuthorisation(long targetId)
		{
			return await Execute(async user =>
				await nests.AuthorisedToFollow(user.Id, targetId));
		}

		[HttpPost("{targetId}/report")]
		public async Task<IActionResult> ReportUser(long targetId, [FromBody] AccountReportManifest report)
		{
			// Verify parameters
			if (report == null || !ModelState.IsValid)
			{ return MissingInformation(); }

			return await Execute(async user =>
				await reports.ReportUserAsync(user.Id, targetId, report.ReportType, report.ReportDetails));
		}

		#endregion
	}
}