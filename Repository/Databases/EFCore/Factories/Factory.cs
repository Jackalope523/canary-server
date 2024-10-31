namespace Repository
{
    internal abstract class Factory
    {
        private List<IFactoryObserver> observers;

        #region constructors
        internal Factory() 
        {
            observers = new();
        }
        internal Factory(IFactoryObserver assignee) : this()
        {
            observers.Add(assignee);
        }
        internal Factory(IEnumerable<IFactoryObserver> assignees) : this()
        {
            observers.AddRange(assignees);
        }
        internal Factory(params IFactoryObserver[] assignees) : this()
        {
            observers.AddRange(assignees);
        }
        #endregion

        #region methods
        internal void Assign(IFactoryObserver observer)
        {
            observers.Add(observer);
        }
        internal void Dismiss(IFactoryObserver observer)
        {
            observers.Remove(observer);
        }
        internal void NotifyObservers(Entity entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        #endregion
    }
}
