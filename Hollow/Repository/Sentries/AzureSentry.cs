using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Text;

namespace Repository
{
    public class AzureSentry : IDatabaseSentry, IBlobStorageSentry
    {
        public AzureSentry()
        {

        }
        public static void SeedDatabase()
        {
            using AzureContext context = new();

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

        #region Database
        public T ExecuteRead<T>(Func<QueryContext, T> read)
        {
            using (AzureContext context = new())
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
            using (AzureContext context = new())
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
            using (AzureContext context = new())
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
            using (AzureContext context = new())
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
            using (AzureContext context = new())
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
            return new Discussion(new AzureContext());
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
        #endregion

        #region Blob Storage
        public async Task UploadBlob(string accountName, string containerName, string blobName, string blobContents)
        {
            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                        accountName,
                                                        containerName);

            // Get a credential and create a client object for the blob container.
            BlobContainerClient containerClient = new(new Uri(containerEndpoint),new DefaultAzureCredential());

            try
            {
                // Create the container if it does not exist.
                await containerClient.CreateIfNotExistsAsync();

                // Upload text to a new block blob.
                byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    await containerClient.UploadBlobAsync(blobName, stream);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<Stream> DownloadBlobAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlobAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BlobExistsAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> ListBlobsAsync(string containerName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}


