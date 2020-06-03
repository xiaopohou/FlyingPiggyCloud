using Sentry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using QingzhenyunApis.Exceptions;
using System.Windows.Threading;
using System.Reflection;

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
            var _ = Uri.IsWellFormedUriString("http://www.baidu.com", UriKind.Absolute);
            var assembly = Assembly.GetAssembly(typeof(Uri));
            if (assembly != null)
            {
                var uriType = assembly.GetType("System.Uri");
                if (uriType != null)
                {
                    uriType.InvokeMember("s_IdnScope", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic, null, null, new object[] { UriIdnScope.None });
                    uriType.InvokeMember("s_IriParsing", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic, null, null, new object[] { false });
                }
            }
        }

    }
}
