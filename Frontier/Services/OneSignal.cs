using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Boundaries;
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

        public async Task PushNotification(string notificationId, NotificationGroup notificationGroup, string title, string message, string collpaseId = "")
        {
            var notification = new Notification(appId: appId)
            {
                Headings = new StringMap(en: title),
                Contents = new StringMap(en: message),
                TargetChannel = Notification.TargetChannelEnum.Push,
                ChannelForExternalUserIds = "push",
                IncludeExternalUserIds = new() { notificationId }, // Deprecated is a mistake

                Filters = new()
                {
                    new(field: "tag", key: notificationGroup.GetString(), value: "1", relation: Filter.RelationEnum.Equal),
                },

                CollapseId = collpaseId,
            };

            var response = await instance.CreateNotificationAsync(notification);

            log.LogInformation($"Notification sent to {notificationId}");
            log.LogInformation($"Delivered to {response.Recipients} recipients");
        }
    }
}
