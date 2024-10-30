using Repository;
using Repository.Entities;
using System;

namespace Repository
{
    internal class Factory
    {
        List<IFactoryObserver> observers = new();

        internal void Subscribe(IFactoryObserver observer)
        {
            observers.Add(observer);
        }
        internal void Unsubscribe(IFactoryObserver observer)
        {
            observers.Remove(observer);
        }
        internal void NotifyObservers(User entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Gathering entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(UserRelationship entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(GatheringLink entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(SnapshotLink entity)
         {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(UserReport entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(GatheringReport entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(SnapshotReport entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Snapshot entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Telegram entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Subscription entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Penalty entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Banner entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(BannerLink entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(GuestClearance entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
        internal void NotifyObservers(Feedback entity)
        {
            foreach (IFactoryObserver observer in observers)
            {
                observer.Notify(entity);
            }
        }
    }
}
