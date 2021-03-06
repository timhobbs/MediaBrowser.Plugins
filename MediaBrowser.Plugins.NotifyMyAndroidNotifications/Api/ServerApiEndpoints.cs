﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Plugins.NotifyMyAndroidNotifications.Configuration;
using ServiceStack;


namespace MediaBrowser.Plugins.NotifyMyAndroidNotifications.Api
{
    [Route("/Notification/NotifyMyAndroid/Test/{UserID}", "POST", Summary = "Tests NotifyMyAndroid")]
    public class TestNotification : IReturnVoid
    {
        [ApiMember(Name = "UserID", Description = "User Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserID { get; set; }
    }

    class ServerApiEndpoints : IRestfulService
    {
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;

        public ServerApiEndpoints(ILogManager logManager, IHttpClient httpClient)
        {
            _logger = logManager.GetLogger(GetType().Name);
            _httpClient = httpClient;
        }

        private NotifyMyAndroidOptions GetOptions(String userID)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.MediaBrowserUserId, userID, StringComparison.OrdinalIgnoreCase));
        }

        public object Post(TestNotification request)
        {
            var options = GetOptions(request.UserID);

            var parameters = new Dictionary<string, string>
            {
                {"apikey", options.Token},
                {"event", "Test Notification"},
                {"description", "This is a test notification from MediaBrowser"},
                {"application", "Media Browser"}
            };

            _logger.Debug("NotifyMyAndroid <TEST> to {0}", options.Token);

            return _httpClient.Post(new HttpRequestOptions { Url = "https://www.notifymyandroid.com/publicapi/notify" }, parameters);
        }
    }
}
