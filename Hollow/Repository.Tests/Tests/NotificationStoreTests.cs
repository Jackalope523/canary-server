
using Core.Boundaries;
using Shared;
using Xunit.Abstractions;

namespace Repository.Tests.Tests
{
    public class NotificationStoreTests: IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static NotificationStore store = new NotificationStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        

        public NotificationStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            

        }
        public void Dispose()
        {
           
        }

        [Fact]
        public Task<List<Note>> GetNotesAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public Task<(DeviceType DeviceType, string DeviceToken)> GetUserSubscriptionAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public Task<bool> SaveNoteAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public Task<bool> SubscribeUserAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public Task<bool> UnsubscribeUserAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}
