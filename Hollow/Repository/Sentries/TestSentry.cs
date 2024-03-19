using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository
{
    public class TestSentry : Sentry
    {      
        public TestSentry() 
        {

        }

        public static void SeedDatabase()
        {
            using TestContext context = new();

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

        public override T ExecuteRead<T>(Func<QueryContext, T> read)
        {
            using (TestContext context = new())
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
        
        public override void ExecuteWrite(Action<QueryContext> write)
        {
            using (TestContext context = new())
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

        public async override Task<T> ExecuteReadAsync<T>(Func<QueryContext, Task<T>> read)
        {
            using (TestContext context = new())
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
   
        public async override Task ExecuteWriteAsync(Action<QueryContext> write)
        {
            using (TestContext context = new())
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
        public async override Task ExecuteWriteAsync(Func<QueryContext,Task> write)
        {
            using (TestContext context = new()) 
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

        public override Discussion BeginDiscussion()
        {
            return new Discussion(new TestContext());
        }

        public override void DiscussWrite(Action<QueryContext> write, Discussion discussion)
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

        public override void EndDiscussion(Discussion toEnd)
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

        public async override Task EndDiscussionAsync(Discussion toEnd)
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
