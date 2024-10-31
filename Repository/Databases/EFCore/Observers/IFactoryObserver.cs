using Repository.Entities;

namespace Repository
{
    internal interface IFactoryObserver
    {
        public void Notify(Entity entity);
    }
}
