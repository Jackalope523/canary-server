using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class ReaperObserver : IFactoryObserver
    {
        public List<Entity> Blacklist {  get; private set; }

        internal ReaperObserver()
        {
            Blacklist = new();
        }
        
        public void Notify(Entity entity)
        {
            Blacklist.Add(entity);
        }
        public void Notify(params Entity[] entities)
        {
            Blacklist.AddRange(entities);
        }
        public void Notify(IEnumerable<Entity> entities)
        {
            Blacklist.AddRange(entities);
        }
        public void Deleted(Entity entity)
        {
            for (int i = 0; i < Blacklist.Count; i++)
            {
                if (Blacklist[i].Id == entity.Id)
                {
                    Blacklist.RemoveAt(i);
                    break;
                }
            }
        }
        public void Reap(EFCoreSentry sentry)
        {
            Blacklist.Sort(new DeletePriorityComparer());
            foreach (Entity entity in Blacklist) 
            {
                sentry.ExecuteWrite(ctx => ctx.Remove(entity));
            }
        }
    }
}
