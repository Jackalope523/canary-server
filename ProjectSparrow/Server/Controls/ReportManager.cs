using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Boundaries;
using Server.Entities;
using Shared;

namespace Server.Controls
{
	internal class ReportManager : AbstractManager, IReportOperations
	{
		public ReportManager(CoreTerminal terminal) : base(terminal) { }

        public async Task ReportUserAsync(Guid userID, Guid targetID,
            UserReportType reportType, string reportDetails)
        {
            Event occuringEvent = new(Events.FindCurrentEventForUser(targetID));
            
            Reports.ReportUser(userID, occuringEvent.Id, targetID, reportType, reportDetails);

            // Compute user's standing
            var user = await GetUser(targetID);
            var status = await user.Reported();

            // Check if host should be punished
            if (user.AccountStatus != status)
            {
                Accounts.UpdateUser(targetID, new() { ("AccountStatus", status) });
            }
        }

        public async Task ReportEventAsync(Guid userID, Guid eventID, Guid hostId,
            EventReportType reportType, string reportDetails)
        {
            var targetEvent = await GetEvent(eventID);
            Reports.ReportEvent(userID, eventID, hostId, reportType, reportDetails);

            // Check if action is to be taken
            if (await targetEvent.Reported())
            {
                // Threshold hit, end event
                await Terminal.EventManager.EndEventAsync(targetEvent.Host.Id, eventID);

                // Compute host's standing
                var user = await GetUser(targetEvent.Host.Id);
                var status = await user.EventReported();

                // Check if host should be punished
                if (user.AccountStatus != status)
                {
                    Accounts.UpdateUser(user.Id, new() { ("AccountStatus", status) });
                }
            }
        }

        internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)>
            GetAllReportsAsync(Guid userID)
        {
            return Reports.GetReportsForUser(userID);
        }

        internal async Task<List<EventReport>> GetEventReportsAsync(Guid eventID)
        {
            return Reports.GetReportsForEvent(eventID);
        }
    }
}

