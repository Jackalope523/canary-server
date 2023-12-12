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
		public ReportDirector(CoreTerminal terminal) : base(terminal) { }

        public async Task ReportUserAsync(Guid userID, Guid targetID,
            UserReportType reportType, string reportDetails)
        {
            Event occuringEvent = new(await Events.FindCurrentEventForUserAsync(targetID));
            
            Reports.ReportUserAsync(userID, occuringEvent.Id, targetID, reportType, reportDetails);

            // Compute user's standing
            var user = await GetUser(targetID);
            var status = await user.Reported();

            // Check if host should be punished
            if (user.AccountStatus != status)
            {
                Accounts.UpdateUserAsync(targetID, new() { ("AccountStatus", status) });
            }
        }

        public async Task ReportEventAsync(Guid userID, Guid eventID, Guid hostId,
            EventReportType reportType, string reportDetails)
        {
            var targetEvent = await GetEvent(eventID);
            Reports.ReportEventAsync(userID, eventID, hostId, reportType, reportDetails);

            // Check if action is to be taken
            if (await targetEvent.Reported())
            {
                // Threshold hit, end event
                await Terminal.EventDirector.EndEventAsync(targetEvent.Host.Id, eventID);

                // Compute host's standing
                var user = await GetUser(targetEvent.Host.Id);
                var status = await user.EventReported();

                // Check if host should be punished
                if (user.AccountStatus != status)
                {
                    Accounts.UpdateUserAsync(user.Id, new() { ("AccountStatus", status) });
                }
            }
        }

        internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)>
            GetAllReportsAsync(Guid userID)
        {
            return await Reports.GetReportsForUserAsync(userID);
        }

        internal async Task<List<EventReport>> GetEventReportsAsync(Guid eventID)
        {
            return await Reports.GetReportsForEventAsync(eventID);
        }
    }
}

