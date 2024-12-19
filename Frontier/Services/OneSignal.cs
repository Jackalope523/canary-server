using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Notifications;
using OneSignalApi.Api;
using OneSignalApi.Client;
using OneSignalApi.Model;

namespace Frontier.Services
{
    public class OneSignalService : INotificationService
    {
        private static ILogger log;
        private static DefaultApi instance;

        private static string appId;
        private static string appToken;

        public static void Initialise(ILogger logger, string apiAccessToken, string apiAppId)
        {
            log = logger;

            appId = apiAppId;
            appToken = apiAccessToken;
        }

        public async Task PushNotification(Guid userNotificationId, CanaryNotification notification)
        {
            if (userNotificationId.Equals(Guid.Empty))
            {
                log.LogWarning("Tried to push notification to empty user.\nTitle {title}\nBody {body}", notification.Body, notification.Body);
                return;
            }

            // The onesignal .net pkg is a mess, do not press further into this until it is updated

            var payload = new
            {
                app_id = appId,
                headings = new { en = notification.Title },
                contents = new { en = notification.Body },
                target_channel = "push",
                include_aliases = new { external_id = new[] { userNotificationId.ToString() } },
                app_url = notification.AppUrl,
                collapse_id = notification.CollapseId,
            };

            string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            
            using HttpClient client = new();

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + appToken);

            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://onesignal.com/api/v1/notifications", requestContent);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                log.LogInformation("Notification sent successfully: {info}", responseContent);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                log.LogError("Failed to send notification: {info}", errorContent);
            }
        }
    }
}
