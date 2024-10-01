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

        public async Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message)
        {
            var notification = new Notification(appId: appId)
            {
                Headings = new StringMap(en: title),
                Contents = new StringMap(en: message),
                IncludedSegments = new List<string> { "Subscribed Users" }
            };

            var response = await instance.CreateNotificationAsync(notification);

            log.LogInformation($"Notification created for {response.Recipients} recipients");
        }
    }
}
