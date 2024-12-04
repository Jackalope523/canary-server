namespace Repository
{
    internal class RumorFactory : Factory
    {
        #region constructors
        public RumorFactory(IFactoryObserver observer) : base(observer)
        {

        }

        public RumorFactory(IEnumerable<IFactoryObserver> observers) : base(observers)
        {

        }

        public RumorFactory(params IFactoryObserver[] observers) : base(observers)
        {

        }
        #endregion

        internal Rumor Create(User author, RumoredGathering rumoredGathering)
        {
            return Create(new Rumor
            {
                AuthorId = author.Id,
                RumoredGatheringId = rumoredGathering.Id,
            });
        }
    }
}
