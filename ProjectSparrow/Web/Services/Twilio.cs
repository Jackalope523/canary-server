using System.Threading.Tasks;

namespace Web.Services
{
	public class Twilio : ISMSService
	{
		public Task SendSMSAsync(string phoneNumber, string message)
		{
			throw new System.NotImplementedException();
		}
	}
}
