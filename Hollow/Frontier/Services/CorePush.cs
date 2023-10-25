using System;
using System.Net.Http;
using System.Threading.Tasks;

using CorePush.Apple;
using CorePush.Google;

namespace Frontier.Services
{
	public class CorePush : INotificationService
	{
		#region Apn Sender Settings

		private static string APNBundleId, APNP8PrivateKey, APNP8PrivateKeyId, APNTeamId;
		private static ApnServerType APNServerType = ApnServerType.Development;

		#endregion

		#region FCM Sender Settings

		private static string FCMSenderId, FCMServerKey;

		#endregion

		private static readonly HttpClient http = new();

		public static void Initialise(string apnBundleId, string apnP8PrivateKey,
			string apnP8PrivateKeyId, string apnTeamId, ApnServerType apnServerType,
			string fcmSenderId, string fcmServerKey)
		{
			APNBundleId = apnBundleId;
			APNP8PrivateKey = apnP8PrivateKey;
			APNP8PrivateKeyId = apnP8PrivateKeyId;
			APNTeamId = apnTeamId;
			APNServerType = apnServerType;

			FCMSenderId = fcmSenderId;
			FCMServerKey = fcmServerKey;
		}

		public async Task PushNotification(string deviceToken, string title, string message)
		{
			if (deviceToken.StartsWith("")) // TODO Change to proper form
			{
				await SendApnNotificationAsync(deviceToken, title, message);
			}
			else
			{
				await SendFcmNotificationAsync(deviceToken, title, message);
			}
		}

		private async Task SendApnNotificationAsync(string apnDeviceToken, string title, string message)
		{
			var settings = new ApnSettings
			{
				AppBundleIdentifier = APNBundleId,
				P8PrivateKey = APNP8PrivateKey,
				P8PrivateKeyId = APNP8PrivateKeyId,
				TeamId = APNTeamId,
				ServerType = APNServerType,
			};

			var apn = new ApnSender(settings, http);
			var payload = new AppleNotification(
				Guid.NewGuid(),
				message,
				title);

			var response = await apn.SendAsync(payload, apnDeviceToken);
		}

		private async Task SendFcmNotificationAsync(string fcmReceiverToken, string title, string body)
		{
			var settings = new FcmSettings
			{
				SenderId = FCMSenderId,
				ServerKey = FCMServerKey
			};

			var fcm = new FcmSender(settings, http);
			var payload = new
			{
				notification = new
				{
					title,
					body
				}
			};

			var response = await fcm.SendAsync(fcmReceiverToken, payload);
		}
	}
}
