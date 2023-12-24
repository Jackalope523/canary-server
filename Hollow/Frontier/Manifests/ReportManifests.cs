using System;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace Frontier.Manifests
{
    public class AccountReportManifest
    {
        [Required]
        public UserReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }

    public class EventReportManifest
    {
        [Required]
        public EventReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }
}

