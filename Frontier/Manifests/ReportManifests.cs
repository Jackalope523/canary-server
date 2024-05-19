using System;

using System.ComponentModel.DataAnnotations;

namespace Frontier.Manifests
{
	public class AccountReportManifest
    {
        [Required]
        public UserReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }

    public class GatheringReportManifest
    {
        [Required]
        public GatheringReportType ReportType { get; set; }

        public string ReportDetails { get; set; }
    }
}

