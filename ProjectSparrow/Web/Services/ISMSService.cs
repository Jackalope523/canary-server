using System.Threading.Tasks;

namespace Web.Services
{
	public interface ISMSService
	{
		Task SendSMSAsync(string phoneNumber, string message);
	}
}
