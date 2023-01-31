using System.Threading.Tasks;

namespace Web.Services
{
	public class SendGrid : IEmailService
	{
		public Task SendEmailAsync(string email, string subject, string body)
		{
			throw new System.NotImplementedException();
		}
	}
}
