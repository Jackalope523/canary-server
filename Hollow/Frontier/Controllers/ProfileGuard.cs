using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Shared;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("profile")]
    public class ProfileGuard : AbstractGuard
	{
		#region Initialisation

		public ProfileGuard(ILogger logger,
			UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, IKeyOperations keyOperations,
			IMediaOperations mediaOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(logger,
				identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				disciplineOperations, keyOperations,
				mediaOperations, notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetProfile(ulong targetIdentification)
		{
			return await Execute(async user =>
			{
				// Retrieve profile
				var profile = await profiles.GetUserProfileAsync(user.Id, targetIdentification);

				return Ok(profile);
			});
		}

		[HttpGet("{targetIdentification}/nest")]
        public async Task<IActionResult> GetNest(ulong targetIdentification)
		{
			return await Execute(async user =>
			{
				// Retrieve nest
				var nest = await profiles.GetUserNestAsync(user.Id, targetIdentification);

				return Ok(nest);
			});
		}

		[HttpPost("{targetIdentification}")]
		public async Task<IActionResult> RateUser(ulong targetIdentification, [FromBody] AccountRatingManifest details)
        {
			// Verify parameters
            if (details != null && !ModelState.IsValid)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await profiles.RateUserAsync(user.Id, targetIdentification, details.Rating);
			});
		}

		[HttpGet("{targetIdentification}/activity")]
		public async Task<IActionResult> GetUserActivity(ulong targetIdentification)
		{
			return await Execute(async user =>
			{
				// Retrieve activity
				var activity = await profiles.GetUserActivityAsync(user.Id, targetIdentification);

				return Ok(activity);
			});
		}

		[HttpGet("activity")]
		public async Task<IActionResult> GetFriendActivity()
		{
			return await Execute(async user =>
			{
				// Retrieve activity
				var activity = await profiles.GetFriendActivityAsync(user.Id);

				return Ok(activity);
			});
		}

		[HttpGet("following")]
        public async Task<IActionResult> GetFollowed()
		{
			return await Execute(async user =>
			{
				// Retrieve all users that the current user is following
				var followedUsers = await profiles.GetFollowedUsersAsync(user.Id);

				return Ok(followedUsers);
			});
		}

		[HttpPost("following")]
		public async Task<IActionResult> FollowUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				// Follow other user
				await profiles.FollowUserAsync(user.Id, info.TargetId);
			});
		}

		[HttpPut("following")]
		public async Task<IActionResult> UnfollowUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				// Unfollow other user
				await profiles.UnfollowUserAsync(user.Id, info.TargetId);
			});
		}

		[HttpGet("blocked")]
		public async Task<IActionResult> GetBlocked()
		{
			return await Execute(async user =>
			{
				// Retrieve all users that the current user is blocking
				var blockedUsers = await profiles.GetBlockedUsersAsync(user.Id);

				return Ok(blockedUsers);
			});
		}

		[HttpPost("blocked")]
		public async Task<IActionResult> BlockUser([FromBody] TargetManifest info)
		{
			// Verify parameters
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				// Block other user
				await profiles.BlockUserAsync(user.Id, info.TargetId);
			});
		}

		[HttpPut("blocked")]
		public async Task<IActionResult> UnblockUser([FromBody] TargetManifest info)
		{
			if (info == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				// Unblock other user
				await profiles.UnblockUserAsync(user.Id, info.TargetId);
			});
		}

		[HttpPost("{targetIdentification}/report")]
		public async Task<IActionResult> ReportUser(ulong targetId, [FromBody] AccountReportManifest report)
		{
			// Verify parameters
			if (report == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await reports.ReportUserAsync(user.Id, targetId, report.ReportType, report.ReportDetails);
			});
		}

		#endregion
	}
}