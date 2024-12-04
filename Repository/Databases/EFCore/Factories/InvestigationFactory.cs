namespace Repository
{
    internal class InvestigationFactory : Factory
    {
        #region constructors
        public InvestigationFactory(IFactoryObserver observer) : base(observer)
        {

        }

        public InvestigationFactory(IEnumerable<IFactoryObserver> observers) : base(observers)
        {

        }

        public InvestigationFactory(params IFactoryObserver[] observers) : base(observers)
        {

        }
        #endregion

        internal Investigation Create(User author, RumoredGathering rumoredGathering)
        {
            return Create(new Investigation
            {
                InvestigatorId = author.Id,
                RumoredGatheringId = rumoredGathering.Id,
            });
        }
    }
}
