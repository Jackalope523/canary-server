using System;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace Frontier.Models
{
    public class AccountReportModel
    {
        [Required]
        public UserReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }

    public class EventReportModel
    {
        [Required]
        public EventReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }
}

