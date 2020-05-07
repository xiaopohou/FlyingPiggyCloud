using Sentry;
using System;

namespace SixCloudCore.Controllers
{
    internal static class SentryAgent
    {
        internal static void Submit(this Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }

        //internal static SentryScopeInfo AttachExtraInfo(this Exception exception, string extraKey, object extraValue)
        //{
        //    var x = new SentryScopeInfo(exception);
        //    x.
        //    SentrySdk.WithScope(scope =>
        //    {
        //        scope.SetExtra(extraKey, extraValue);
        //    });
        //}

        //internal sealed class SentryScopeInfo
        //{
        //    internal Action<Scope> scopeCallBack;

        //    internal readonly Exception exception;

        //    internal SentryScopeInfo(Exception exception)
        //    {
        //        this.exception = exception;
        //    }
        //}
    }
}
