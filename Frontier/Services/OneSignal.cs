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

        public static void Initialise(ILogger logger, string apiAccessToken, string apiAppId)
        {
            log = logger;

            appId = apiAppId;

            var appConfig = new Configuration
            {
                BasePath = "https://onesignal.com/api/v1",
                AccessToken = apiAccessToken
            };

            instance = new DefaultApi(appConfig);
        }

        public async Task PushNotification(NotificationProfile userNotificationProfile, CanaryNotification notification)
        {
            // Check valid target
            if (userNotificationProfile.NotificationId.Equals(Guid.Empty))
            {
                log.LogWarning("Tried to push notification to empty user.\nTitle {title}\nBody {body}", notification.Body, notification.Body);
                return;
            }
            // Check user preferences
            if (!notification.CheckEnabled(userNotificationProfile))
            {
                return;
            }

            var notif = new Notification(appId: appId)
            {
                Headings = new StringMap(en: notification.Title),
                Contents = new StringMap(en: notification.Body),
                ChannelForExternalUserIds = "push",
                IncludeExternalUserIds = new() { userNotificationProfile.NotificationId.ToString() }, // Deprecated is a mistake, leave as is

                AppUrl = notification.AppUrl,
                CollapseId = notification.CollapseId,
            };

            await instance.CreateNotificationAsync(notif);
        }
    }
}
