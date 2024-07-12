using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;

namespace Frontier.Controllers
{
    [Route("gathering")]
    public class GatheringGuard : AbstractGuard
	{
		#region Initialisation

		public GatheringGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet("{gatheringId}")]
        public async Task<IActionResult> GetGathering(ulong gatheringId)
        {
			return await Execute(async user =>
			{
				// Retrieve gathering information
				return await gatherings.GetGatheringInformationAsync(user.Id, gatheringId);
			});
        }

        [HttpPost]
        public async Task<IActionResult> CreateGathering([FromBody] GatheringDetailsManifest gatheringDetails)
        {
			// Verify parameters
            if (gatheringDetails == null || !ModelState.IsValid)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				// Create a new gathering
				return await gatherings.CreateGatheringAsync(user.Id,
					gatheringDetails.Name, gatheringDetails.Description,
					gatheringDetails.StartTime,
					gatheringDetails.Latitude, gatheringDetails.Longitude, gatheringDetails.FriendlyLocation,
					gatheringDetails.Radius, gatheringDetails.IsDynamic,
					gatheringDetails.GroupMinimum, gatheringDetails.GroupMaximum,
					await StreamFirstFile());
			});
        }

        [HttpPost("{gatheringId}/edit")]
        public async Task<IActionResult> EditGathering(ulong gatheringId, [FromBody] GatheringDetailsManifest gatheringDetails)
		{
			// Verify parameters
			if (gatheringDetails == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await gatherings.EditGatheringAsync(user.Id, gatheringId,
					gatheringDescription: gatheringDetails.Description ?? "",
					isOpen: gatheringDetails.IsOpen,
					startTime: gatheringDetails.StartTime,
					latitude: gatheringDetails.Latitude, longitude: gatheringDetails.Longitude,
					friendlyLocation: gatheringDetails.FriendlyLocation,
					radius: gatheringDetails.Radius, isDynamic: gatheringDetails.IsDynamic,
					groupMinimum: gatheringDetails.GroupMinimum, groupMaximum: gatheringDetails.GroupMaximum);
			});
		}

		[HttpGet("{gatheringId}/start")]
		public async Task<IActionResult> StartGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Start gathering
				await gatherings.StartGatheringAsync(user.Id, gatheringId);
			});
		}

        [HttpDelete("{gatheringId}/edit")]
        public async Task<IActionResult> EndGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// End an gathering
				await gatherings.EndGatheringAsync(user.Id, gatheringId);
			});
        }

        [HttpDelete("{gatheringId}")]
        public async Task<IActionResult> DeleteGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Delete an gathering
				await gatherings.DeleteGatheringAsync(user.Id, gatheringId);
			});
        }

		[HttpPost("{gatheringId}/survey")]
		public async Task<IActionResult> SurveyGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Join an gathering
				await gatherings.SurveyGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpPut("{gatheringId}/survey")]
		public async Task<IActionResult> UnsurveyGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Join an gathering
				await gatherings.UnsurveyGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpPost("{gatheringId}")]
        public async Task<IActionResult> JoinGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Join an gathering
				await gatherings.JoinGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpPut("{gatheringId}")]
		public async Task<IActionResult> LeaveGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Leave an gathering
				await gatherings.LeaveGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpGet("{gatheringId}/guests")]
		public async Task<IActionResult> GetGuestList(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				return await gatherings.GetGuestListAsync(user.Id, gatheringId);
			});
		}

		[HttpGet("{gatheringId}/invite")]
		public async Task<IActionResult> GetPotentialInvitees(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				return await gatherings.GetPotentialInviteesAsync(user.Id, gatheringId);
			});
        }

		[HttpPost("{gatheringId}/invite")]
		public async Task<IActionResult> InviteUser(ulong inviteeId, ulong gatheringId)
		{
			return await Execute(async user =>
			{
				await gatherings.InviteUserAsync(user.Id, inviteeId, gatheringId);
			});
        }

		[HttpPut("{gatheringId}/guests")]
		public async Task<IActionResult> KickUser(ulong targetId, ulong gatheringId)
		{
			return await Execute(async user =>
			{
				await gatherings.KickUserAsync(user.Id, targetId, gatheringId);
			});
		}

		[HttpPost("{gatheringId}/report")]
		public async Task<IActionResult> ReportGathering(ulong gatheringId, [FromBody] GatheringReportManifest report)
		{
			// Verify parameters
			if (report == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await reports.ReportGatheringAsync(user.Id, gatheringId, report.ReportType, report.ReportDetails);
			});
		}

		[HttpGet("{gatheringId}/snapshots")]
		public async Task<IActionResult> GetGatheringSnapshots(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				return await snapshots.GetGatheringSnapshotsAsync(user.Id, gatheringId);
			});
		}

		[HttpPost("{gatheringId}/snapshots")]
		public async Task<IActionResult> SnapshotGathering(ulong gatheringId)
		{
			// Verify parameters
			if (!ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				return await snapshots.AddSnapshotAsync(user.Id, gatheringId, await StreamFirstFile());
			});
		}

		[HttpPut("{gatheringId}/snapshots")]
		public async Task<IActionResult> RemoveSnapshot(ulong gatheringId, ulong snapshotId)
		{
			return await Execute(async user =>
			{
				await snapshots.RemoveSnapshotAsync(user.Id, snapshotId);
			});
		}

		[HttpPost("{gatheringId}/snapshots/{snapshotId}")]
		public async Task<IActionResult> AcclaimSnapshot(ulong gatheringId, ulong snapshotId, [FromBody] AccountRatingManifest details)
		{
			// Verify parameters
			if (details == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
				await snapshots.AcclaimSnapshotAsync(user.Id, snapshotId, details.Rating);
			});
		}

		#endregion
	}
}