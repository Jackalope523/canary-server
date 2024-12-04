using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class EFCoreRumorStore : QueryStore, IRumorDatabase
    {
        public EFCoreRumorStore(Harbor.Flag flag) : base(flag)
        {
        }

        public async Task ConfirmRumor(long investigatorId, long rumoredGatheringId)
        {
            long id = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Investigations.
                Where(i => i.InvestigatorId == investigatorId && i.RumoredGatheringId == rumoredGatheringId)
                .Select(l => l.Id)
                .SingleOrDefaultAsync());

            Investigation investigation = new()
            {
                Id = id,
                InvestigatorId = investigatorId,
                RumoredGatheringId = rumoredGatheringId,
                Conclusion = Investigation.InvestigationConclusion.Confirm
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Investigations.Update(investigation));
        }

        public async Task<CoreRumor> CreateRumorAsync(long rumoredGatheringId, long authorId, string text, DateTimeOffset time)
        {
            Rumor toCreate = new()
            {
                RumoredGatheringId = rumoredGatheringId,
                AuthorId = authorId,
                Text = text,
                Time = time
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Rumors.Add(toCreate));

            return new CoreRumor(toCreate.Id, toCreate.Text, toCreate.Time);

        }

        public async Task<CoreRumoredGathering> CreateRumoredGatheringAsync(double latitude, double longitude, string friendlyLocation)
        {
            RumoredGathering toCreate = new()
            {
                Location = new CoordinateFactory().Create(longitude, latitude),
                FriendlyLocation = friendlyLocation,
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.RumoredGatherings.Add(toCreate));

            return new CoreRumoredGathering(toCreate.Id, toCreate.Location.Y, toCreate.Location.X, toCreate.FriendlyLocation, toCreate.ConfidenceRating);
        }

        public async Task DenyRumor(long investigatorId, long rumoredGatheringId)
        {
            long id = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Investigations.
                Where(i => i.InvestigatorId == investigatorId && i.RumoredGatheringId == rumoredGatheringId)
                .Select(l => l.Id)
                .SingleOrDefaultAsync());

            Investigation investigation = new()
            {
                Id = id,
                InvestigatorId = investigatorId,
                RumoredGatheringId = rumoredGatheringId,
                Conclusion = Investigation.InvestigationConclusion.Deny
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Investigations.Update(investigation));
        }

        public async Task<UserShard> GetFounderAsync(long rumoredGatheringId)
        {
            long founderId = await storeSentry.ExecuteReadAsync(ctx => 
                                ctx.Rumors.
                                Where(r => r.RumoredGatheringId == rumoredGatheringId).
                                OrderBy(r => r.Time).
                                Select(r => r.Id).
                                FirstAsync());

            return await storeSentry.ExecuteReadAsync(ctx => 
                    ctx.Users.
                    Where(u => u.Id == founderId).
                    Select(u => new UserShard(u.Id, u.Name)).
                    SingleAsync());
        }

        public async Task<CoreRumor> GetRumorAsync(long id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Rumors.
                Where(r => r.Id == id).
                Select(r => new CoreRumor(r.Id, r.Text, r.Time)).
                SingleAsync());
        }

        public async Task<CoreRumoredGathering> GetRumoredGatheringAsync(long id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.RumoredGatherings.
                Where(r => r.Id == id).
                Select(r => new CoreRumoredGathering(r.Id, r.Location.Y, r.Location.X, r.FriendlyLocation, r.ConfidenceRating)).
                SingleAsync());
        }

        public async Task<List<CoreRumor>> GetRumorsAboutAsync(long rumoredGatheringId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Rumors.
                Where(r => r.RumoredGatheringId == rumoredGatheringId).
                Select(r => new CoreRumor(r.Id, r.Text, r.Time)).
                ToListAsync());
        }

        public async Task<List<CoreRumor>> GetRumorsByAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Rumors.
                Where(r => r.AuthorId == userId).
                Select(r => new CoreRumor(r.Id, r.Text, r.Time)).
                ToListAsync());
        }

        public async Task<List<CoreRumor>> GetRumorsByCompanionsOfAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
                ctx.UserRelationships.
                Where(r => r.SelfId == userId && r.Type == UserRelationship.UserLinkType.Appreciate).
                Join(
                    ctx.UserRelationships.
                    Where(r => r.OtherId == userId && r.Type == UserRelationship.UserLinkType.Appreciate),
                    x => x.SelfId,
                    y => y.OtherId,
                    (x,y) => x.OtherId
                ).
                Join(
                    ctx.Rumors,
                    c => c,
                    r => r.AuthorId,
                    (c,r) => new CoreRumor(r.Id, r.Text, r.Time)
                ).
                ToListAsync());
        }

        public async Task<List<(CoreRumor, string)>> GetWallRumorsAsync(long userId)
        {
            var result = await storeSentry.ExecuteReadAsync(ctx =>
               ctx.UserRelationships.
               Where(r => r.SelfId == userId && r.Type == UserRelationship.UserLinkType.Appreciate).
               Join(
                   ctx.UserRelationships.
                   Where(r => r.OtherId == userId && r.Type == UserRelationship.UserLinkType.Appreciate),
                   x => x.SelfId,
                   y => y.OtherId,
                   (x, y) => x.OtherId
               ).
               Join(
                   ctx.Rumors,
                   c => c,
                   r => r.AuthorId,
                   (c, r) => new { r.Id, r.RumoredGatheringId, r.Text, r.Time }
               ).
               Join(
                   ctx.RumoredGatherings,
                   r => r.RumoredGatheringId,
                   g => g.Id,
                   (r, g) => new { r.Id, r.Text, r.Time, g.FriendlyLocation }
                ).
               ToListAsync());

            return result.Select(x => (new CoreRumor(x.Id, x.Text, x.Time), x.FriendlyLocation)).ToList();
        }

        public async Task HardDeleteRumorAsync(long id)
        {
            Discussion deleteDiscussion = storeSentry.BeginDiscussion();

            storeSentry.DiscussWrite(ctx => ctx.RumorReports.Where(r => r.RumorId == id).ExecuteDelete(), deleteDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Rumors.Remove(new Rumor { Id = id }), deleteDiscussion);

            await storeSentry.EndDiscussionAsync(deleteDiscussion);
        }

        public async Task HardDeleteRumoredGatheringAsync(long id)
        {
            Discussion deleteDiscussion = storeSentry.BeginDiscussion();

            List<long> rumorIds = await storeSentry.ExecuteReadAsync(ctx => 
                                    ctx.Rumors.
                                    Where(r => r.RumoredGatheringId == id).
                                    Select(r => r.Id).
                                    ToListAsync());

            storeSentry.DiscussWrite(ctx => ctx.RumorReports.Where(r => rumorIds.Contains(r.RumorId)).ExecuteDelete(), deleteDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Rumors.Where(r => rumorIds.Contains(r.Id)).ExecuteDelete(), deleteDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Investigations.Where(l => l.RumoredGatheringId == id).ExecuteDelete(), deleteDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.RumoredGatherings.Remove(new RumoredGathering { Id = id }), deleteDiscussion);

            await storeSentry.EndDiscussionAsync(deleteDiscussion);
        }

        public async Task SoftDeleteRumorAsync(long id)
        {
            Discussion updateDiscussion = storeSentry.BeginDiscussion();

            storeSentry.DiscussWrite(ctx =>
                ctx.RumorReports.
                Where(r => r.RumorId == id).
                ExecuteUpdate(setter => setter.SetProperty(l => l.SoftDeleted, true)),
                updateDiscussion);

            Rumor r = new() { Id = id, SoftDeleted = true };
            storeSentry.DiscussWrite(ctx => ctx.Rumors.Attach(r), updateDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(r).Property(nameof(r.SoftDeleted)).IsModified = true, updateDiscussion);

            await storeSentry.EndDiscussionAsync(updateDiscussion);
        }

        public async Task SoftDeleteRumoredGatheringAsync(long id)
        {
            Discussion updateDiscussion = storeSentry.BeginDiscussion();

            List<long> rumorIds = await storeSentry.ExecuteReadAsync(ctx =>
                                    ctx.Rumors.
                                    Where(r => r.RumoredGatheringId == id).
                                    Select(r => r.Id).
                                    ToListAsync());

            storeSentry.DiscussWrite(ctx => 
                ctx.RumorReports.
                Where(r => rumorIds.Contains(r.RumorId)).
                ExecuteUpdate(setter => setter.SetProperty(l => l.SoftDeleted, true)), 
                updateDiscussion);

            storeSentry.DiscussWrite(ctx => 
                ctx.Rumors.
                Where(r => rumorIds.Contains(r.Id)).
                ExecuteUpdate(setter => setter.SetProperty(l => l.SoftDeleted, true)),
                updateDiscussion);

            storeSentry.DiscussWrite(ctx => 
                ctx.Investigations.
                Where(l => l.RumoredGatheringId == id).
                ExecuteUpdate(setter => setter.SetProperty(l => l.SoftDeleted, true)),
                updateDiscussion);

            RumoredGathering r = new() { Id = id, SoftDeleted = true };
            storeSentry.DiscussWrite(ctx => ctx.RumoredGatherings.Attach(r), updateDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(r).Property(nameof(r.SoftDeleted)).IsModified = true, updateDiscussion);

            await storeSentry.EndDiscussionAsync(updateDiscussion);
        }

        public async Task UpdateRumorAsync(long id, List<(string Property, object Value)> edits)
        {
            Discussion updateDiscussion = storeSentry.BeginDiscussion();

            Rumor r = new() { Id = id };

            storeSentry.DiscussWrite(ctx => ctx.Rumors.Attach(r), updateDiscussion);

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case nameof(r.Text):
                        r.Text = (string)Value;
                        break;
                    case nameof(r.Time):
                        r.Time = (DateTimeOffset)Value;
                        break;
                    default:
                        throw new InvalidInputException("Property named \"" + Property + "\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(r).Property(Property).IsModified = true, updateDiscussion);
            }
            await storeSentry.EndDiscussionAsync(updateDiscussion);
        }

        public async Task UpdateRumoredGatheringAsync(long id, List<(string Property, object Value)> edits)
        {
            Discussion updateDiscussion = storeSentry.BeginDiscussion();

            RumoredGathering r = new() { Id = id };

            storeSentry.DiscussWrite(ctx => ctx.RumoredGatherings.Attach(r), updateDiscussion);

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case nameof(r.FriendlyLocation):
                        r.FriendlyLocation = (string)Value;
                        break;
                    case "Location":
                        var Location = ((double, double))Value;
                        r.Location = new CoordinateFactory().Create(Location.Item2, Location.Item1);
                        break;
                    case nameof(r.ConfidenceRating):
                        r.ConfidenceRating = (int)Value;
                        break;
                    default:
                        throw new InvalidInputException("Property named \"" + Property + "\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(r).Property(Property).IsModified = true, updateDiscussion);
            }
            await storeSentry.EndDiscussionAsync(updateDiscussion);
        }
    }
}
