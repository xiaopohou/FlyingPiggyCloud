using QingzhenyunApis.Exceptions;
using Sentry;
using SixCloud.Core.Controllers;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core
{
    public static class Core
    {
        public static void Initialize()
        {
            if (LocalProperties.AccentColor != default)
            {
                ColorSetter.AccentColor = LocalProperties.AccentColor.Value;
            }
            if (LocalProperties.ForegroundColor != default)
            {
                ColorSetter.ForegroundColor = LocalProperties.ForegroundColor.Value;
            }
            if (LocalProperties.BackgroundColor != default)
            {
                ColorSetter.BackgroundColor = LocalProperties.BackgroundColor.Value;
            }

            LangSetter();
        }

        public static void LangSetter(string lang = "zh-CN")
        {
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();
            var requestedCulture = $"pack://application:,,,/SixCloud.Core.LocalizationResources;component/{lang}.xaml";

            var resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));

            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

        }

        static Core()
        {
            DisableIriSupport();
            LibVLCSharp.Shared.Core.Initialize();
            SentrySdk.Init("https://22d1bd82c69c4667a8604cca40c8f4b3@sentry.qiecdn.com/3");
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
