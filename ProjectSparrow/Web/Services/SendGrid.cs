using System;
using System.Threading.Tasks;
using SendGrid;
using Twilio.TwiML.Messaging;
using Twilio.Types;

namespace Web.Services
{
	public class SendGridService : IEmailService
	{
		private static bool isInitialised = false;

		public SendGridService()
		{
			if (!isInitialised)
			{
				Initialise();
			}
		}

		public static void Initialise()
		{
			isInitialised = true;
			//SendGridClient.Init(Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"), Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_TOKEN"));
		}

		public Task SendEmailAsync(string email, string subject, string body)
		{
			Console.WriteLine($"Email to {email} [{subject}]: {body}");
			return Task.FromResult(0);
		}
	}
}
