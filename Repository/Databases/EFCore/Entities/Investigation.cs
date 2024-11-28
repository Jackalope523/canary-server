namespace Repository
{
    public class Investigation : Entity
    {
        public enum InvestigationConclusion { Confirm, Deny }

        public long InvestigatorId { get; set; }
        public long RumoredGatheringId { get; set; }
        public InvestigationConclusion Conclusion { get; set; } = DefaultConclusion;

        // Navigation Properties
        public User? Investigator { get; set; }
        public RumoredGathering? RumoredGathering { get; set; }

        // Default Values
        public static InvestigationConclusion DefaultConclusion = InvestigationConclusion.Deny;
    }
}
