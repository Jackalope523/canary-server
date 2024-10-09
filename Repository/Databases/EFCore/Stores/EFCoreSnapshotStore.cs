using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EFCoreSnapshotStore : QueryStore, ISnapshotDatabase
    {   
        public EFCoreSnapshotStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task<SnapshotShard> AddSnapshotAsync(ulong gatheringId, ulong posterId, DateTimeOffset timePosted)
        { 
            Snapshot toAdd = new() 
            { 
                GatheringId = gatheringId, 
                OwnerId = posterId, 
                PostedAt = timePosted
            };
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Snapshots.Add(toAdd));

            string ownerName = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == posterId).
                Select(u => u.Name).
                SingleAsync());

            return new SnapshotShard ( toAdd.Id, toAdd.GatheringId, new UserShard(toAdd.OwnerId, ownerName), toAdd.PostedAt, 0);
        }

        public async Task<List<SnapshotShard>> GenerateColumnForUserAsync(ulong id, DateTimeOffset depthCharge, DateTimeOffset lastDepthCharge)
        {
            // Get List of Companions.
            Task<List<ulong>> appreciating = storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.SelfId == id && l.Type == UserRelationship.UserLinkType.Appreciate).Select(l => l.OtherId).
                ToListAsync());

            Task<List<ulong>> appreciatingMe = storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserLinks.
                Where(l => l.OtherId == id && l.Type == UserRelationship.UserLinkType.Appreciate).
                Select(l => l.SelfId).
                ToListAsync());

            List<ulong> owners = (await appreciating).Intersect(await appreciatingMe).Append(id).ToList();

            // Get unseen posts by companions from certain depth.
            List<SnapshotShard> companionSnapshots = await storeSentry.ExecuteReadAsync(ctx =>
              ctx.Snapshots.
              Where(p => owners.Contains(p.OwnerId) && p.PostedAt >= depthCharge && p.PostedAt <= lastDepthCharge).
              Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new SnapshotShard(p.Id, p.GatheringId, new(u.Id, u.Name), p.PostedAt, -1)
               ).ToListAsync());

            List<Task<int>> companionSnapshotsPositiveRatings = new();

            List<ulong> sitesToBeExplored = new();
            List<ulong> previouslyExtractedSnapshots = new();
            foreach (SnapshotShard s in companionSnapshots)
            {
                companionSnapshotsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => 
                    ctx.SnapshotLinks.
                    Where(l => l.SnapshotId == s.Id && l.Type == SnapshotLink.SnapshotLinkType.Appreciate).
                    CountAsync()));


                // Compile unique list if gatherings spanned by companion posts and a list of already loaded posts. 
                if (!sitesToBeExplored.Contains(s.GatheringId)) sitesToBeExplored.Add(s.GatheringId);
                previouslyExtractedSnapshots.Add(s.Id);
            }

            // Get remaining companion posts from same gatherings as others even if outside time range.
            List<SnapshotShard> nettedSnapshots = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Snapshots.
                Where(p => owners.Contains(p.OwnerId) && !previouslyExtractedSnapshots.Contains(p.Id) && sitesToBeExplored.Contains(p.GatheringId)).
                Join(
                  ctx.Users,
                  p => p.OwnerId,
                  u => u.Id,
                  (p, u) => new SnapshotShard(p.Id, p.GatheringId, new(u.Id, u.Name), p.PostedAt, -1)
                ).ToListAsync());

            List<Task<int>> nettedSnapshotsPositiveRatings = new();

            foreach (SnapshotShard s in nettedSnapshots)
            {
                nettedSnapshotsPositiveRatings.Add(storeSentry.ExecuteReadAsync(ctx =>
                    ctx.SnapshotLinks.
                    Where(l => l.SnapshotId == s.Id && l.Type == SnapshotLink.SnapshotLinkType.Appreciate).
                    CountAsync()));
            }

            for (int i = 0; i < companionSnapshots.Count; i++)
            {
                companionSnapshots[i] = companionSnapshots[i] with { Acclaim = await companionSnapshotsPositiveRatings[i] };
            }

            for (int i = 0; i < nettedSnapshots.Count; i++)
            {
                nettedSnapshots[i] = nettedSnapshots[i] with { Acclaim = await nettedSnapshotsPositiveRatings[i] };
            }

            return companionSnapshots.Concat(nettedSnapshots).ToList();

        }

        public async Task<SnapshotShard> GetSnapshotAsync(ulong id)
        {
            Task<int> appreciates = storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.SnapshotId == id && l.Type == SnapshotLink.SnapshotLinkType.Appreciate).CountAsync());

            SnapshotShard snapshot = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.Snapshots.
            Where(p => p.Id == id).
            Select(p => new SnapshotShard(p.Id, p.GatheringId, new UserShard(p.OwnerId, null), p.PostedAt, 0)).
            SingleAsync());

            Task<string> name = storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.Id == snapshot.User.Id).
                Select(u => u.Name).
                SingleAsync());

            return snapshot with { User = new UserShard(snapshot.User.Id, await name), Acclaim = await appreciates };
        }

        public async Task<List<SnapshotShard>> GetSnapshotsByUserAsync(ulong id)
        {
            List<SnapshotShard> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Snapshots.Where(p => p.OwnerId == id).
                 Select(a => new SnapshotShard(a.Id, a.GatheringId, new UserShard(a.OwnerId, null), a.PostedAt, 0)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(snapshots.Count);
            List<Task<string>> authorNames = new(snapshots.Count);
            for (int i = 0; i < snapshots.Count; i++)
            {
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.SnapshotId == snapshots[i].Id && l.Type == SnapshotLink.SnapshotLinkType.Appreciate).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == snapshots[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < snapshots.Count; i++)
            {
                snapshots[i] = snapshots[i] with { Acclaim = ups[i], User = new UserShard(snapshots[i].User.Id, names[i]) };
            }

            return snapshots;
        }

        public async Task AcclaimSnapshotAsync(ulong postId, ulong voterId)
        {
            SnapshotLink toAdd = new()
            {
                UserId = voterId,
                SnapshotId = postId,
                Time = DateTimeOffset.UtcNow,
                Type = SnapshotLink.SnapshotLinkType.Appreciate
            };

            ulong id = await storeSentry.ExecuteReadAsync(ctx =>
                        ctx.SnapshotLinks.
                        Where(l => l.UserId == voterId && l.SnapshotId == postId).
                        Select(l => l.Id).
                        SingleOrDefaultAsync());

            if (id != 0)
            {
                toAdd.Id = id;
            }

            await storeSentry.ExecuteWriteAsync(ctx => ctx.SnapshotLinks.Update(toAdd));
        }

        public async Task DeleteSnapshotAsync(ulong postId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Snapshots.Remove(new Snapshot { Id = postId }));
        }

        public async Task DeleteSnapshotAcclaimAsync(ulong postId, ulong voterId)
        {
            Func<CanaryContext, Task> query = EF.CompileAsyncQuery(
                (CanaryContext ctx) =>
                ctx.SnapshotLinks.
                Where(l => l.UserId == voterId && l.SnapshotId == postId).
                ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(query);
        }

        public async Task<List<SnapshotShard>> GetSnapshotsForGatheringAsync(ulong id)
        {
            List<SnapshotShard> snapshots = await storeSentry.ExecuteReadAsync(ctx =>
                 ctx.Snapshots.Where(p => p.GatheringId == id).
                 Select(a => new SnapshotShard(a.Id, a.GatheringId, new UserShard(a.OwnerId, null), a.PostedAt, 0)).
                 ToListAsync());

            List<Task<int>> positiveRatings = new(snapshots.Count);
            List<Task<string>> authorNames = new(snapshots.Count);
            for (int i = 0; i < snapshots.Count; i++)
            {            
                positiveRatings.Add(storeSentry.ExecuteReadAsync(ctx => ctx.SnapshotLinks.Where(l => l.SnapshotId == snapshots[i].Id && l.Type == SnapshotLink.SnapshotLinkType.Appreciate).CountAsync()));
                authorNames.Add(storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == snapshots[i].User.Id).Select(u => u.Name).SingleAsync()));
            }

            int[] ups = await Task.WhenAll(positiveRatings);
            string[] names = await Task.WhenAll(authorNames);

            for (int i = 0; i < snapshots.Count; i++)
            {
                snapshots[i] = snapshots[i] with { Acclaim = ups[i], User = new UserShard(snapshots[i].User.Id, names[i]) };
            }

            return snapshots;
        }

        public async Task RemoveSnapshotAsync(ulong snapshotId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => ctx.Snapshots.Remove(new Snapshot { Id = snapshotId }));
        }

        public async Task RemoveSnapshotAcclaimAsync(ulong snapshotId, ulong voterId)
        {
            Func<CanaryContext, Task> query = EF.CompileAsyncQuery(
                (CanaryContext ctx) =>
                ctx.SnapshotLinks.
                Where(l => l.UserId == voterId && l.SnapshotId == snapshotId).
                ExecuteDelete());

            await storeSentry.ExecuteWriteAsync(query);
        }
    }
}
