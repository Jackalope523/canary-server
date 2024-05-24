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

		[HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetNest(ulong targetIdentification)
		{
			return await Execute(async user =>
				await nests.GetUserNestAsync(user.Id, targetIdentification));
		}

		[HttpGet("companions")]
        public async Task<IActionResult> GetCompanions()
        {
            return await Execute(async user =>
                await nests.GetCompanionsAsync(user.Id));
        }

        [HttpGet("appreciating")]
        public async Task<IActionResult> GetAppreciated()
		{
			return await Execute(async user =>
				await nests.GetAppreciatedUsersAsync(user.Id));
		}

		[HttpPost("appreciating")]
		public async Task<IActionResult> AppreciateUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await nests.AppreciateUserAsync(user.Id, info.TargetId));
		}

		[HttpPut("appreciating")]
		public async Task<IActionResult> UnappreciateUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await nests.UnappreciateUserAsync(user.Id, info.TargetId));
		}

		[HttpGet("blocked")]
		public async Task<IActionResult> GetBlocked()
		{
			return await Execute(async user =>
				await nests.GetBlockedUsersAsync(user.Id));
		}

		[HttpPost("blocked")]
		public async Task<IActionResult> BlockUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await nests.BlockUserAsync(user.Id, info.TargetId));
		}

		[HttpPut("blocked")]
		public async Task<IActionResult> UnblockUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
				await nests.UnblockUserAsync(user.Id, info.TargetId));
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