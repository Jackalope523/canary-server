namespace Repository
{
    internal class RumorReportFactory : Factory
    {
        #region constructors
        public RumorReportFactory(IFactoryObserver observer) : base(observer)
        {

        }

        public RumorReportFactory(IEnumerable<IFactoryObserver> observers) : base(observers)
        {

        }

        public RumorReportFactory(params IFactoryObserver[] observers) : base(observers)
        {

        }
        #endregion

        internal RumorReport Create(User user, Rumor rumor)
        {
            return Create(new RumorReport
            {
                UserId = user.Id,
                RumorId = rumor.Id,
            });
        }
    }
}