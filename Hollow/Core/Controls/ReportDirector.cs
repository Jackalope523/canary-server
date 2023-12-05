using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
	internal class ReportDirector : AbstractDirector, IReportOperations
	{
		#region Initialisation

		public ReportDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task ReportUserAsync(ulong userId, ulong targetId,
            UserReportType reportType, string reportDetails)
        {
            var user = await GetUserAsync(userId);
            var targetUser = await GetUserAsync(targetId);
            var occuringEvent = (await targetUser.CurrentEvent.Value()) ?? Event.None;

            // Verify user can report
            Try(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            await Reports.ReportUserAsync(userId, occuringEvent.Id, targetUser.Id, reportType, reportDetails);

            // Compute user's standing
            var status = await targetUser.Reported();

            // Check if user should be punished
            if (targetUser.AccountStatus != status)
            {
                _ = Accounts.UpdateUserAsync(targetUser.Id, new() { (nameof(UserShard.AccountStatus), status) });
            }
        }

        public async Task ReportEventAsync(ulong userId, ulong eventId,
            EventReportType reportType, string reportDetails)
        {
            var user = await GetUserAsync(userId);
            var targetEvent = await GetEventAsync(eventId);

            // Verify user can report
            Try(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            await Reports.ReportEventAsync(user.Id, targetEvent.Id, targetEvent.Host.Id, reportType, reportDetails);

            // Check if action is to be taken
            if (await targetEvent.Reported())
            {
                var host = await GetUserAsync(targetEvent.Host.Id);

                // Threshold hit, end event
                _ = Terminal.EventDirector.EndEventAsync(host.Id, eventId);

                // Compute host's standing
                var status = await host.EventReported();

                // Check if host should be punished
                if (host.AccountStatus != status)
                {
                    _ = Accounts.UpdateUserAsync(host.Id, new() { (nameof(UserShard.AccountStatus), status) });
                }
            }
        }

		#endregion

		#region Favours

        internal async Task<List<Penalty>> RequestPenaltiesForUserAsync(User user)
        {
            return await Reports.GetPenaltiesForUserAsync(user.Id);
        }

        internal async Task<bool> PenaliseUserAsync(User user, Penalty penalty)
        {
            return await Reports.PenaliseUserAsync(user.Id, penalty);
        }

		internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)>
            RequestAllReportsAsync(User user)
        {
            return await Reports.GetReportsForUserAsync(user.Id);
        }

        internal async Task<List<EventReport>> RequestEventReportsAsync(Event @event)
        {
            return await Reports.GetReportsForEventAsync(@event.Id);
        }

		#endregion
	}
}

