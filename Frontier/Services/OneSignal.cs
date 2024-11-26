using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Notifications;
using Newtonsoft.Json;
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

        public async Task PushNotification(Guid userNotificationId, CanaryNotification notification)
        {
            if (userNotificationId.Equals(Guid.Empty))
            {
                log.LogWarning("Tried to push notification to empty user.\nTitle {title}\nBody {body}", notification.Body, notification.Body);
                return;
            }

            var notif = new Notification(appId: appId)
            {
                Headings = new StringMap(en: notification.Title),
                Contents = new StringMap(en: notification.Body),
                TargetChannel = Notification.TargetChannelEnum.Push,
                ChannelForExternalUserIds = "push",
                IncludeExternalUserIds = new() { userNotificationId.ToString() }, // Deprecated is a mistake, leave as is

                Filters = new()
                {
                    new(field: "tag", key: notification.Group.GetString(), value: "1", relation: Filter.RelationEnum.Equal),
                },

                AppUrl = notification.AppUrl,
                CollapseId = notification.CollapseId,
            };

            log.LogError("Short-circuiting notification {notification}", notif.ToJson());

            return;

            await instance.CreateNotificationAsync(notif);
        }
    }
}
