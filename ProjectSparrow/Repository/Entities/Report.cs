using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Report
    {
        public enum ReportType { Inappropriate, Spam, Misleading, Promotion, Rude, HateSpeech, Harassment, ViolentBehaviour, PhysicalAssault, SexualAssault }

        public Guid Id { get; init; }

        public Guid SelfId { get; init; }
        internal User Self { get; init; } // Navigation Property

        public Guid OtherId { get; init; }
        internal User Other { get; init; } // Navigation Property

        public Guid EventId { get; init; } 
        internal Event Event { get; init; } // Navigation Property

        public ReportType Type { get; init; }
        public string Notes { get; init; }

        public static ReportType ToReportType(UserReportType type)
        {

        }

        public static ReportType ToReportType(EventReportType type)
        {

        }


    }
}
