using QingzhenyunApis.Exceptions;
using Sentry;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core
{
    public static class Core
    {
        public static void Initialize()
        {

        }

        static Core()
        {
            DisableIriSupport();
            LibVLCSharp.Shared.Core.Initialize();
            SentrySdk.Init("https://aa9303eba050450187a9c04653e74be5@o387540.ingest.sentry.io/5222970");
            Application.Current.DispatcherUnhandledException += (sender, e) =>
            {
                e.Exception.ToSentry().TreatedBy(nameof(DispatcherUnhandledExceptionEventHandler)).Submit();
            };
        }

        private static void DisableIriSupport()
        {
            bool _ = Uri.IsWellFormedUriString("http://www.baidu.com", UriKind.Absolute);
            Assembly assembly = Assembly.GetAssembly(typeof(Uri));
            if (assembly != null)
            {
                Type uriType = assembly.GetType("System.Uri");
                if (uriType != null)
                {
                    uriType.InvokeMember("s_IdnScope", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic, null, null, new object[] { UriIdnScope.None });
                    uriType.InvokeMember("s_IriParsing", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic, null, null, new object[] { false });
                }
            }
        }

    }
}
