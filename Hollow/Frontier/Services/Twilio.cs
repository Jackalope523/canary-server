using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Frontier.Services
{
	public class TwilioService : ISMSService
	{
		private static string senderPhoneNumber = "";

		public static void Initialise(string accountID, string token, string phoneNumber)
		{
			TwilioClient.Init(accountID, token);

			senderPhoneNumber = phoneNumber;
		}

		public async Task SendSMSAsync(string phoneNumber, string message)
		{
			if (phoneNumber[0] == '+')
			{
				await MessageResource.CreateAsync(
					from: new Twilio.Types.PhoneNumber(senderPhoneNumber),
					to: new Twilio.Types.PhoneNumber(phoneNumber),
					body: message);
			}
			else
			{
				Console.WriteLine($"SMS to {phoneNumber}: {message}");
			}
		}
	}
}
