using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.IO;

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
        public async Task<IActionResult> CreateGathering([FromForm] GatheringCreationManifest gatheringDetails)
        {
			// Verify parameters
            if (gatheringDetails == null || !ModelState.IsValid ||
				gatheringDetails.Image == null || gatheringDetails.Image.Length == 0)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
            {
                using var stream = new MemoryStream();
                await gatheringDetails.Image.CopyToAsync(stream);

                // Create a new gathering
                return await gatherings.CreateGatheringAsync(user.Id,
                    gatheringDetails.Name, gatheringDetails.Description,
                    gatheringDetails.StartTime,
                    gatheringDetails.Latitude, gatheringDetails.Longitude, gatheringDetails.FriendlyLocation,
                    gatheringDetails.Radius, gatheringDetails.IsDynamic,
                    gatheringDetails.GroupMinimum, gatheringDetails.GroupMaximum,
                    stream);
            });
        }

        [HttpPost("{gatheringId}/edit")]
        public async Task<IActionResult> EditGathering(ulong gatheringId, [FromForm] GatheringEditManifest gatheringDetails)
		{
			// Verify parameters
			if (gatheringDetails == null)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
			{
                using var stream = new MemoryStream();
				if (gatheringDetails.Image != null && gatheringDetails.Image.Length > 0)
				{
					await gatheringDetails.Image.CopyToAsync(stream);
				}

				await gatherings.EditGatheringAsync(user.Id, gatheringId,
					gatheringName: gatheringDetails.Name,
					gatheringDescription: gatheringDetails.Description,
					startTime: gatheringDetails.StartTime,
					latitude: gatheringDetails.Latitude, longitude: gatheringDetails.Longitude,
					friendlyLocation: gatheringDetails.FriendlyLocation,
					radius: gatheringDetails.Radius, isDynamic: gatheringDetails.IsDynamic,
					groupMinimum: gatheringDetails.GroupMinimum, groupMaximum: gatheringDetails.GroupMaximum,
					heroImage: stream);
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
				// End gathering
				await gatherings.TerminateGatheringAsync(user.Id, gatheringId);
			});
        }

        [HttpDelete("{gatheringId}")]
        public async Task<IActionResult> DeleteGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Delete gathering
				await gatherings.DeleteGatheringAsync(user.Id, gatheringId);
			});
        }

        [HttpPost("{gatheringId}/visibility")]
        public async Task<IActionResult> HideGathering(ulong gatheringId, bool hide)
		{
			return await Execute(async user =>
			{
				await gatherings.ChangeGatheringVisibilityAsync(user.Id, gatheringId, hide);
			});
        }

		[HttpPost("{gatheringId}/survey")]
		public async Task<IActionResult> SurveyGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Watch gathering
				await gatherings.WatchGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpPut("{gatheringId}/survey")]
		public async Task<IActionResult> UnsurveyGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Unwatch gathering
				await gatherings.UnwatchGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpPost("{gatheringId}")]
        public async Task<IActionResult> JoinGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Join gathering
				await gatherings.JoinGatheringAsync(user.Id, gatheringId);
			});
		}

		[HttpGet("{gatheringId}/checkin/{latitude},{longitude}")]
        public async Task<IActionResult> CheckInToGathering(ulong gatheringId, float latitude, float longitude)
		{
			return await Execute(async user =>
			{
				// Check in to gathering
				await gatherings.CheckInToGatheringAsync(user.Id, latitude, longitude);
			});
		}

		[HttpPut("{gatheringId}")]
		public async Task<IActionResult> LeaveGathering(ulong gatheringId)
		{
			return await Execute(async user =>
			{
				// Leave gathering
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

		[HttpPost("{gatheringId}/invite/{inviteeId}")]
		public async Task<IActionResult> InviteUser(ulong gatheringId, ulong inviteeId)
		{
			return await Execute(async user =>
			{
				await gatherings.InviteUserAsync(user.Id, inviteeId, gatheringId);
			});
        }

		[HttpPut("{gatheringId}/guests/{userId}")]
		public async Task<IActionResult> KickUser(ulong gatheringId, ulong userId)
		{
			return await Execute(async user =>
			{
				await gatherings.KickUserAsync(user.Id, userId, gatheringId);
			});
		}

		[HttpGet("{gatheringId}/authorisation/start")]
		public async Task<IActionResult> CheckStartAuthorisation(ulong gatheringId)
		{
			return await Execute(async user => await gatherings.AuthorisedToStart(user.Id, gatheringId));
		}

		[HttpGet("{gatheringId}/authorisation/join")]
		public async Task<IActionResult> CheckJoinAuthorisation(ulong gatheringId)
		{
			return await Execute(async user => await gatherings.AuthorisedToJoin(user.Id, gatheringId));
		}

		[HttpGet("{gatheringId}/authorisation/upload")]
		public async Task<IActionResult> CheckUploadAuthorisation(ulong gatheringId)
		{
			return await Execute(async user => await gatherings.AuthorisedToUpload(user.Id, gatheringId));
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

		[HttpGet("{gatheringId}/snapshots,{targetId}")]
		public async Task<IActionResult> GetGallery(ulong gatheringId, ulong targetId)
		{
			return await Execute(async user =>
			{
				return await snapshots.GetGalleryAsync(user.Id, targetId, gatheringId);
			});
		}

		[HttpPost("{gatheringId}/snapshots")]
		public async Task<IActionResult> SnapshotGathering(ulong gatheringId, [FromForm] SnapshotManifest snapshot)
        {
            // Verify parameters
            if (snapshot == null || !ModelState.IsValid ||
                snapshot.Image == null || snapshot.Image.Length == 0)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
            {
                using var stream = new MemoryStream();
                await snapshot.Image.CopyToAsync(stream);

                return await snapshots.AddSnapshotAsync(user.Id, gatheringId, stream);
			});
		}

		[HttpPut("{gatheringId}/snapshots/{snapshotId}")]
		public async Task<IActionResult> RemoveSnapshot(ulong gatheringId, ulong snapshotId)
		{
			return await Execute(async user =>
			{
				await snapshots.DeleteSnapshotAsync(user.Id, snapshotId);
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
				await snapshots.AcclaimSnapshotAsync(user.Id, snapshotId, details.Action);
			});
		}

		[HttpPost("{gatheringId}/snapshots/{snapshotId}/report")]
		public async Task<IActionResult> ReportSnapshot(ulong gatheringId, ulong snapshotId, [FromBody] SnapshotReportManifest report)
		{
			// Verify parameters
			if (report == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async user =>
            {
                await reports.ReportSnapshotAsync(user.Id, snapshotId, report.ReportType, report.ReportDetails);
            });
		}

		#endregion
	}
}