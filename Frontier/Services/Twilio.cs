using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Core.Boundaries;

namespace Frontier.Services
{
	public class TwilioService : ISMSService
	{
		private static ILogger log;

		private static string messagingServiceSid = "";

		public static void Initialise(ILogger logger, string accountId, string accountToken, string messagingService)
		{
			log = logger;
			messagingServiceSid = messagingService;

			TwilioClient.Init(accountId, accountToken);
		}

		public async Task SendSMSAsync(string phoneNumber, string message)
		{
			log.LogInformation("SMS to {phoneNumber}: {message}", phoneNumber, message);
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
	}
}
