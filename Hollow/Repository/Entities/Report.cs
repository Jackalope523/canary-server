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
            return type switch
            {
                UserReportType.physical_assault => ReportType.PhysicalAssault,
                UserReportType.rude => ReportType.Rude,
                UserReportType.harassment => ReportType.Harassment,
                UserReportType.violent_behaviour => ReportType.ViolentBehaviour,
                UserReportType.sexual_assault => ReportType.SexualAssault,
                UserReportType.hate_speech => ReportType.HateSpeech,
                _ => ReportType.PhysicalAssault,
            };
        }
        public static ReportType ToReportType(EventReportType type)
        {
            return type switch
            {
                EventReportType.promotion => ReportType.Promotion,
                EventReportType.spam => ReportType.Spam,
                EventReportType.inappropriate => ReportType.Inappropriate,
                EventReportType.misleading => ReportType.Misleading,
                _ => ReportType.PhysicalAssault,
            };
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
