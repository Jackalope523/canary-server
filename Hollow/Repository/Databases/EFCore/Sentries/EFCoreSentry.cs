using Microsoft.EntityFrameworkCore;
using Shared;
using static Repository.Harbor;

namespace Repository
{
    internal class EFCoreSentry : IDatabaseSentry
    {
        private readonly Func<QueryContext> initializeContext;
        internal EFCoreSentry(Flag flag)
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
        internal static void SeedDatabase()
        {
            using AzureSQLContext context = new();

            UserFactory userFactory = new();
            EventFactory eventFactory = new();
            EtchingFactory etchingFactory = new();
            UserReportFactory userReportFactory = new();
            EventReportFactory eventReportFactory = new();
            PenaltyFactory penaltyFactory = new();
            SubscriptionFactory subscriptionFactory = new();
            NoteFactory noteFactory = new();
            UserLinkFactory userLinkFactory = new();
            EventLinkFactory eventLinkFactory = new();
            PostLinkFactory postLinkFactory = new();

            List<User> users = new();
            for (int i = 0; i < 10; i++)
            {
                users.Add(userFactory.Create());
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            List<Event> events = new();
            for (int i = 0; i < 2; i++)
            {
                events.Add(eventFactory.Create(users[i]));
            }
            context.Events.AddRange(events);
            context.SaveChanges();

            List<Post> etchings = new();
            for (int i = 0; i < 10; i++)
            {
                Event location;
                if (i <= 6) location = events[0];
                else location = events[1];

                etchings.Add(etchingFactory.Create(users[i], location));
            }
            context.Posts.AddRange(etchings);
            context.SaveChanges();
        }

        internal T ExecuteRead<T>(Func<QueryContext, T> read)
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

        internal void ExecuteWrite(Action<QueryContext> write)
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

        internal async Task<T> ExecuteReadAsync<T>(Func<QueryContext, Task<T>> read)
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

        internal async Task ExecuteWriteAsync(Action<QueryContext> write)
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
        internal async Task ExecuteWriteAsync(Func<QueryContext, Task> write)
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

        internal Discussion BeginDiscussion()
        {
            return new Discussion(initializeContext());
        }

        internal void DiscussWrite(Action<QueryContext> write, Discussion discussion)
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

        internal void EndDiscussion(Discussion toEnd)
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

        internal async Task EndDiscussionAsync(Discussion toEnd)
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


