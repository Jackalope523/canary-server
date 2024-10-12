using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class InternalUtilityStore : QueryStore
    { 
        public InternalUtilityStore(Harbor.Flag flag) : base(flag)
        {
        }

        internal async Task<UserShard> GetHostShard(long id)
        {
            UserShard hostShard = new UserShard(0, "DeletedUser");
            if (id != 0)
            {
                hostShard = await storeSentry.ExecuteReadAsync(ctx =>
                                ctx.Users.
                                Where(u => u.Id == id).
                                Select(u => new UserShard(u.Id, u.Name)).
                                SingleAsync());
            }
            return hostShard;
        }

        internal async Task populateHostNames(List<CoreGathering> coreGatherings)
        {
            coreGatherings.Sort((x, y) => x.Host.Id.CompareTo(y.Host.Id));

            List<long> hostIds = coreGatherings.Select(g => g.Id).ToList();
            List<UserShard> hosts = await storeSentry.ExecuteReadAsync(ctx =>
                                        ctx.Users.
                                        Where(u => hostIds.Contains(u.Id)).
                                        Select(u => new UserShard(u.Id, u.Name)).
                                        ToListAsync());

            hosts.Sort((x, y) => x.Id.CompareTo(y.Id));

            for (int i = 0; i < coreGatherings.Count; i++)
            {
                coreGatherings[i] = coreGatherings[i] with { Host = hosts[i] };
            }
        }
    }
}
