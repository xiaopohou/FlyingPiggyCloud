using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QingzhenyunApis.Exceptions
{
    public static class SentryAgent
    {
        internal static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static void SetUser(User user, object userEntity)
        {
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = user;
                scope.SetExtra("UserInformation", userEntity);
            });
        }

        public static SentryScopeInfo ToSentry(this Exception exception)
        {
            return new SentryScopeInfo(exception);
        }

        public static SentryScopeInfo AttachExtraInfo(this SentryScopeInfo sentryScopeInfo, string extraKey, object extraValue)
        {
            sentryScopeInfo.Extras[extraKey] = extraValue;
            return sentryScopeInfo;
        }

        public static SentryScopeInfo AttachTag(this SentryScopeInfo sentryScopeInfo, string tagKey, string tagValue)
        {
            sentryScopeInfo.Tags[tagKey] = tagValue;
            return sentryScopeInfo;
        }

        public static SentryScopeInfo TreatedBy(this SentryScopeInfo sentryScopeInfo, string byWhom)
        {
            return sentryScopeInfo.AttachTag(nameof(TreatedBy), byWhom);
        }

        public static void Submit(this Exception exception)
        {
            exception.ToSentry().Submit();
        }

        public static void Submit(this SentryScopeInfo sentryScopeInfo)
        {
            SentrySdk.WithScope(scope =>
            {

                if (sentryScopeInfo.Tags.Keys.Any())
                {
                    scope.SetTags(sentryScopeInfo.Tags);
                }

                if (sentryScopeInfo.Extras.Keys.Any())
                {
                    scope.SetExtras(sentryScopeInfo.Extras);
                }

                SentrySdk.CaptureException(sentryScopeInfo.Exception);
            });
        }
    }

    public sealed class SentryScopeInfo
    {
        internal Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();

        internal Dictionary<string, object> Extras { get; } = new Dictionary<string, object>();

        internal Exception Exception { get; }

        internal SentryScopeInfo(Exception exception)
        {
            Exception = exception;
        }
    }
}
