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
            Event occuringEvent = new(Events.FindCurrentEventForUser(targetId));
            
            Reports.ReportUser(userId, occuringEvent.Id, targetId, reportType, reportDetails);

            // Compute user's standing
            var user = await GetUser(targetId);
            var status = await user.Reported();

            // Check if host should be punished
            if (user.AccountStatus != status)
            {
                Accounts.UpdateUser(targetId, new() { (nameof(UserShard.AccountStatus), status) });
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
                // Threshold hit, end event
                await Terminal.EventDirector.EndEventAsync(targetEvent.Host.Id, eventId);

                // Compute host's standing
                var user = await GetUser(targetEvent.Host.Id);
                var status = await user.EventReported();

                // Check if host should be punished
                if (user.AccountStatus != status)
                {
                    Accounts.UpdateUser(user.Id, new() { (nameof(UserShard.AccountStatus), status) });
                }
            }
        }

		#endregion

		#region Favours

		internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)>
            GetAllReportsAsync(ulong userId)
        {
            return Reports.GetReportsForUser(userId);
        }

        internal async Task<List<EventReport>> GetEventReportsAsync(ulong eventId)
        {
            return Reports.GetReportsForEvent(eventId);
        }

		#endregion
	}
}

