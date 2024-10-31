namespace Repository
{
    internal class ReaperObserver : IFactoryObserver
    {
        List<Entity> touchedTables;

        internal ReaperObserver()
        {
            touchedTables = new();
        }
        
        public void Notify(Entity entity)
        {
            touchedTables.Add(entity);
        }
        public void Reap(EFCoreSentry sentry)
        {
            sentry.ExecuteWrite(ctx => ctx.RemoveRange(touchedTables));
        }
        public List<long> GetBlacklist()
        {
            return touchedTables.Select(e => e.Id).ToList();
        }
    }
}
