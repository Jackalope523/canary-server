using System.Threading.Tasks;

namespace Frontier.Services
{
	public interface ISMSService
	{
		Task SendSMSAsync(string phoneNumber, string message);
	}
}
