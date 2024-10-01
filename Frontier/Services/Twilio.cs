using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Core.Boundaries;

namespace Frontier.Services
{
	public class TwilioService : ISMSService
	{
		private static string senderPhoneNumber = "";
		private static ILogger log;

		public static void Initialise(ILogger logger, string accountId, string accountToken, string phoneNumber)
		{
			log = logger;

			// TwilioClient.Init(accountId, accountToken);

			senderPhoneNumber = phoneNumber;
		}

		public async Task SendSMSAsync(string phoneNumber, string message)
		{
			log.LogInformation("SMS to {phoneNumber}: {message}", phoneNumber, message);

			if (phoneNumber[0] == '+')
			{
				await MessageResource.CreateAsync(
					from: new Twilio.Types.PhoneNumber(senderPhoneNumber),
					to: new Twilio.Types.PhoneNumber(phoneNumber),
					body: message);
			}
		}
	}
}
