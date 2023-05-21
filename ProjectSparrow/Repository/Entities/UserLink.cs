namespace Repository.Entities
{
    public class UserLink : Link
    {
        public enum UserLinkType { Follow, Block, RateUp, RateDown, Rude, HateSpeech, Harassment, ViolentBehaviour, PhysicalAssault, SexualAssault }

        public Guid OtherId { get; init; }
        internal User Other { get; init; }
        public UserLinkType Type { get; set; }

    }
}
