using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class BlobTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private static readonly AzureFileStore sentry = new();

        public BlobTests(ITestOutputHelper testOutputHelper) 
        {
            _testOutputHelper = testOutputHelper;

        }
        public void Dispose()
        {
            
        }

        [Fact]
        public Task UploadBlob_SUCCESS()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task DownloadBlobAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task DeleteBlobAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task BlobExistsAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task ListBlobsAsync_SUCCESS()
        {
            throw new NotImplementedException();
        }
    }
}
