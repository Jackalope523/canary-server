using Microsoft.EntityFrameworkCore;

using static Repository.Harbor;

namespace Repository
{
    internal class EFCoreSentry : IDatabaseSentry
    {
        private readonly Func<QueryContext> initializeContext;
        public EFCoreSentry(Flag flag)
        {
            switch (flag)
            {
                case Flag.Development:
                    initializeContext = () => new SQLiteContext();
                    break;

                case Flag.Production:
                    initializeContext = () => new AzureSQLContext();
                    break;

                default:
                    throw new UnsupportedHarborFlagException();
            }
        }
        public static void SeedDatabase()
        {
            using AzureSQLContext context = new();

            UserFactory userFactory = new();
            GatheringFactory gatheringFactory = new();
            SnapshotFactory snapshotFactory = new();
            UserReportFactory userReportFactory = new();
            GatheringReportFactory gatheringReportFactory = new();
            PenaltyFactory penaltyFactory = new();
            SubscriptionFactory subscriptionFactory = new();
            NoteFactory noteFactory = new();
            UserLinkFactory userLinkFactory = new();
            GatheringLinkFactory gatheringLinkFactory = new();
            PostLinkFactory postLinkFactory = new();

            List<User> users = new();
            for (int i = 0; i < 10; i++)
            {
                users.Add(userFactory.Create());
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            List<Gathering> gatherings = new();
            for (int i = 0; i < 2; i++)
            {
                gatherings.Add(gatheringFactory.Create(users[i]));
            }
            context.Gatherings.AddRange(gatherings);
            context.SaveChanges();

            List<Post> snapshots = new();
            for (int i = 0; i < 10; i++)
            {
                Gathering location;
                if (i <= 6) location = gatherings[0];
                else location = gatherings[1];

                snapshots.Add(snapshotFactory.Create(users[i], location));
            }
            context.Posts.AddRange(snapshots);
            context.SaveChanges();
        }

        public T ExecuteRead<T>(Func<QueryContext, T> read)
        {
            using (QueryContext context = initializeContext())
            {
                try
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    return read.Invoke(context);
                }
                catch (Exception ex)
                {
                    throw new DatabaseReadException(ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public void ExecuteWrite(Action<QueryContext> write)
        {
            using (QueryContext context = initializeContext())
            {
                try
                {
                    write.Invoke(context);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new DatabaseWriteException(ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public async Task<T> ExecuteReadAsync<T>(Func<QueryContext, Task<T>> read)
        {
            using (QueryContext context = initializeContext())
            {
                try
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    return await read.Invoke(context);
                }
                catch (Exception ex)
                {
                    throw new DatabaseReadException(ex);
                }
                finally
                {
                    await context.DisposeAsync();
                }
            }
        }

        public async Task ExecuteWriteAsync(Action<QueryContext> write)
        {
            using (QueryContext context = initializeContext())
            {
                try
                {
                    write.Invoke(context);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new DatabaseWriteException(ex);
                }
                finally
                {
                    await context.DisposeAsync();
                }
            }
        }
        public async Task ExecuteWriteAsync(Func<QueryContext, Task> write)
        {
            using (QueryContext context = initializeContext())
            {
                try
                {
                    await write.Invoke(context);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new DatabaseWriteException(ex);
                }
                finally
                {
                    await context.DisposeAsync();
                }
            }
        }

        public Discussion BeginDiscussion()
        {
            return new Discussion(initializeContext());
        }

        public void DiscussWrite(Action<QueryContext> write, Discussion discussion)
        {
            try
            {
                write.Invoke(discussion.SharedContext);
            }
            catch (Exception ex)
            {
                discussion.EndNow();
                throw new DatabaseWriteException(ex);
            }
        }

        public void EndDiscussion(Discussion toEnd)
        {
            try
            {
                toEnd.End();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
        }

        public async Task EndDiscussionAsync(Discussion toEnd)
        {
            try
            {
                await toEnd.EndAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseWriteException(ex);
            }
        }
    }
}


