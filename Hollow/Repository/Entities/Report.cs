using Shared;

namespace Repository
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
        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }
      
        public static ReportType ToReportType(UserReportType type)
        {
            switch (type)
            {
                case UserReportType.physical_assault:
                    return ReportType.PhysicalAssault;
                case UserReportType.rude:
                    return ReportType.Rude;
                case UserReportType.harassment:
                    return ReportType.Harassment;
                case UserReportType.violent_behaviour:
                    return ReportType.ViolentBehaviour;
                case UserReportType.sexual_assault:
                    return ReportType.SexualAssault;
                case UserReportType.hate_speech:
                    return ReportType.HateSpeech;
                default:
                    return ReportType.PhysicalAssault;
               
                    
            }
        }
        public static ReportType ToReportType(EventReportType type)
        {
            switch (type)
            {
                case EventReportType.promotion:
                    return ReportType.Promotion;
                case EventReportType.spam:
                    return ReportType.Spam;
                case EventReportType.inappropriate:
                    return ReportType.Inappropriate;
                case EventReportType.misleading:
                    return ReportType.Misleading;
                default:
                    return ReportType.PhysicalAssault;
            }
        }
        public static UserReportType ToUserReportType(ReportType type)
        {
            switch (type)
            {
                case ReportType.PhysicalAssault:
                    return UserReportType.physical_assault;
                case ReportType.Rude:
                    return UserReportType.rude;
                case ReportType.Harassment:
                    return UserReportType.harassment;
                case ReportType.ViolentBehaviour:
                    return UserReportType.violent_behaviour;
                case ReportType.SexualAssault:
                    return UserReportType.sexual_assault;
                case ReportType.HateSpeech:
                    return UserReportType.hate_speech;
                default:
                    return UserReportType.physical_assault;


            }
        }
        public static EventReportType ToEventReportType(ReportType type)
        {
            switch (type)
            {
                case ReportType.Promotion:
                    return EventReportType.promotion;
                case ReportType.Spam:
                    return EventReportType.spam;
                case ReportType.Inappropriate:
                    return EventReportType.inappropriate;
                case ReportType.Misleading:
                    return EventReportType.misleading;
                default:
                    return EventReportType.spam;
            }
        }


    }
}
