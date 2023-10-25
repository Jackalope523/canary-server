using System.Threading.Tasks;

namespace Frontier.Services
{
	public interface INotificationService
	{
		Task PushNotification(string deviceToken, string title, string message);
	}
}
