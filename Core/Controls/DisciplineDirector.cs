using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class DisciplineDirector : AbstractDirector, IDisciplineOperations
	{
		#region Initialisation

		public DisciplineDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task ReportUserAsync(long userId, long targetId,
            UserReportType reportType, string reportDetails)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);
            var occuringGathering = await targetUser.CurrentGathering;

            // Verify user can report
            Verify(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            if (occuringGathering.Equals(Gathering.None))
            {
                await Reports.ReportUserAsync(userId, targetUser.Id, Time, reportType, reportDetails);
            }
            else
            {
                await Reports.ReportUserAsync(userId, occuringGathering.Id, targetUser.Id, Time, reportType, reportDetails);
            }

            // Compute user's standing
            var status = await targetUser.Reported();

            // Check if user should be punished
            if (targetUser.AccountStatus != status)
            {
                _ = Accounts.UpdateUserAsync(targetUser.Id, new() { (nameof(CoreUser.AccountStatus), status) });
            }
        }

        public async Task ReportGatheringAsync(long userId, long gatheringId,
            GatheringReportType reportType, string reportDetails)
        {
            var user = await GetUserAsync(userId);
            var targetGathering = await GetGatheringAsync(gatheringId);

            // Verify user can report
            Verify(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            await Reports.ReportGatheringAsync(user.Id, targetGathering.Id, Time, reportType, reportDetails);

            // Check if action is to be taken
            if (await targetGathering.Reported())
            {
                var host = await GetUserAsync(targetGathering.Host.Id);

                // Threshold hit, end gathering
                _ = Terminal.GatheringDirector.TerminateGatheringAsync(host.Id, gatheringId);

                // Compute host's standing
                var status = await host.GatheringReported();

                // Check if host should be punished
                if (host.AccountStatus != status)
                {
                    _ = Accounts.UpdateUserAsync(host.Id, new() { (nameof(CoreUser.AccountStatus), status) });
                }
            }
        }

        public async Task ReportSnapshotAsync(long userId, long snapshotId,
            SnapshotReportType reportType, string reportDetails)
        {
            var user = await GetUserAsync(userId);
            var targetSnapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            User targetUser = new(targetSnapshot.User);

            // Verify user can report
            Verify(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            await Reports.ReportSnapshotAsync(user.Id, targetSnapshot.Id, Time, reportType, reportDetails);

            // Compute user's standing
            var status = await targetUser.Reported();

            // Check if user should be punished
            if (targetUser.AccountStatus != status)
            {
                _ = Accounts.UpdateUserAsync(targetUser.Id, new() { (nameof(CoreUser.AccountStatus), status) });
            }
        }

		#endregion

		#region Favours

        internal async Task<List<PenaltyShard>> RequestPenaltiesForUserAsync(User user)
            => await Reports.GetPenaltiesForUserAsync(user.Id);

        internal async Task PenaliseUserAsync(User user, PenaltyType offense, DateTimeOffset timeOfPenalty)
            => await Reports.PenaliseUserAsync(user.Id, offense, timeOfPenalty);

		internal async Task<(List<UserReport> UserReports, List<GatheringReport> GatheringReports, List<SnapshotReport> SnapshotReports)>
            RequestAllReportsAsync(User user)
            => await Reports.GetReportsForUserAsync(user.Id);

        internal async Task<List<GatheringReport>> RequestGatheringReportsAsync(Gathering gathering)
            => await Reports.GetReportsForGatheringAsync(gathering.Id);

		#endregion
	}
}

