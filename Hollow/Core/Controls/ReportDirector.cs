using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

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
            var user = await GetUser(userId);
            var targetUser = await GetUser(targetId);
            await targetUser.SyncCurrentEvent();
            var occuringEvent = targetUser.CurrentEvent ?? new(0);

            Try(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            Reports.ReportUser(userId, occuringEvent.Id, targetUser.Id, reportType, reportDetails);

            // Compute user's standing
            var status = await targetUser.Reported();

            // Check if user should be punished
            if (targetUser.AccountStatus != status)
            {
                Accounts.UpdateUser(targetUser.Id, new() { (nameof(UserShard.AccountStatus), status) });
            }
        }

        public async Task ReportEventAsync(ulong userId, ulong eventId,
            EventReportType reportType, string reportDetails)
        {
            User user = new(userId);
            var targetEvent = await GetEvent(eventId);

            Try(await user.CanReport(),
                new InvalidUserException("User has a cooldown to report."));

            Reports.ReportEvent(user.Id, targetEvent.Id, targetEvent.Host.Id, reportType, reportDetails);

            // Check if action is to be taken
            if (await targetEvent.Reported())
            {
                var host = await GetUser(targetEvent.Host.Id);

                // Threshold hit, end event
                _ = Terminal.EventDirector.EndEventAsync(host.Id, eventId);

                // Compute host's standing
                var status = await host.EventReported();

                // Check if host should be punished
                if (host.AccountStatus != status)
                {
                    Accounts.UpdateUser(host.Id, new() { (nameof(UserShard.AccountStatus), status) });
                }
            }
        }

		#endregion

		#region Favours

		internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)>
            RequestAllReportsAsync(User user)
        {
            return Reports.GetReportsForUser(user.Id);
        }

        internal async Task<List<EventReport>> RequestEventReportsAsync(Event @event)
        {
            return Reports.GetReportsForEvent(@event.Id);
        }

		#endregion
	}
}

