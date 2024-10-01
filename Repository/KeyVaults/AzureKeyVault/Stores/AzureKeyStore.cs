namespace Repository
{
    public class AzureKeyStore : IKeyDatabase
    {
        AzureKeySentry sentry;

        public AzureKeyStore()
        {
            sentry = new AzureKeySentry();
        }

        public async Task<string> GetHollowOneSignalApiKeyAsync()
        {
            return await sentry.GetSecretAsync("OneSignalApiKey");
        }

        public async Task<string> GetHollowOneSignalAppIdAsync()
        {
            return await sentry.GetSecretAsync("OneSignalAppId");
        }

        public async Task<string> GetHollowTwilioAuthKeyAsync()
        {
            return await sentry.GetSecretAsync("TwilioAccountSID");
        }

        public async Task<string> GetHollowTwilioTokenKeyAsync()
        {
            return await sentry.GetSecretAsync("TwilioAuthToken");
        }

        public async Task<string> GetCanaryMapKeyAsync()
        {
            return await sentry.GetSecretAsync("MapboxSparrowMapToken");
        }
    }
}
