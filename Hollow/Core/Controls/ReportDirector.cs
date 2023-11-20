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
            var targetUser = await GetUser(targetId);
            await targetUser.SyncCurrentEvent();
            var occuringEvent = targetUser.CurrentEvent ?? new(0);
            
            Reports.ReportUser(userId, occuringEvent.Id, targetUser.Id, reportType, reportDetails);

            // Compute user's standing
            var status = await targetUser.Reported();

            // Check if user should be punished
            if (targetUser.AccountStatus != status)
            {
                Accounts.UpdateUser(targetUser.Id, new() { (nameof(UserShard.AccountStatus), status) });
            }
        }

        public async Task ReportEventAsync(ulong userId, ulong eventId, ulong hostId,
            EventReportType reportType, string reportDetails)
        {
            var targetEvent = await GetEvent(eventId);
            Reports.ReportEvent(userId, eventId, hostId, reportType, reportDetails);

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

