using System;
using System.Threading.Tasks;
using Twilio;
using Core;

namespace Frontier.Services
{
	public class TwilioService : ISMSService
	{
		private static ILogger log;
		private static EnvironmentOptions env;

		private static string messagingServiceSid = "";

		public static void Initialise(EnvironmentOptions environment, ILogger logger, string accountId, string accountToken, string messagingService)
		{
			env = environment;
			log = logger;
			messagingServiceSid = messagingService;

			TwilioClient.Init(accountId, accountToken);
		}

		public async Task SendSMSAsync(string phoneNumber, string message)
        {
            log.LogInformation("Want to send SMS to {phoneNumber}", phoneNumber);

            if (env.IsProduction)
            {
                log.LogInformation("Sending SMS to {phoneNumber}: {message}", phoneNumber, message);
                /*
                if (phoneNumber[0] == '+')
                {
                    await MessageResource.CreateAsync(
                        messagingServiceSid: messagingServiceSid,
                        to: new Twilio.Types.PhoneNumber(phoneNumber),
                        body: message);
                }
                */
            }
            else
            {
                log.LogInformation("Dropped SMS to {phoneNumber}: {message}", phoneNumber, message);
            }
		}
	}
}
