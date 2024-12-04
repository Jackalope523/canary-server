namespace Repository
{
    internal class RumoredGatheringFactory : Factory
    {
        #region constructors
        public RumoredGatheringFactory(IFactoryObserver observer) : base(observer)
        {

        }

        public RumoredGatheringFactory(IEnumerable<IFactoryObserver> observers) : base(observers)
        {

        }

        public RumoredGatheringFactory(params IFactoryObserver[] observers) : base(observers)
        {

        }
        #endregion

        internal RumoredGathering Create()
        {
            return Create(new RumoredGathering());
        }
    }
}
