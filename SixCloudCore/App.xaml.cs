using CefSharp;
using CefSharp.Wpf;
using LibVLCSharp.Shared;
using Sentry;
using SixCloudCore.Controllers;
using SixCloudCore.ViewModels;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using QingzhenyunApis.Exceptions;
using System.Windows;
using System.Windows.Threading;
using SixCloudCore.Views.Dialogs;

namespace SixCloudCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeCefSharp();
            Core.Initialize();
            SentrySdk.Init("https://aa9303eba050450187a9c04653e74be5@o387540.ingest.sentry.io/5222970");
            Application.Current.DispatcherUnhandledException += (sender, e) =>
            {
                e.Exception.ToSentry().TreatedBy(nameof(DispatcherUnhandledExceptionEventHandler)).Submit();
            };

            new LoginWebViewModel();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            CefSettings settings = new CefSettings
            {
                // Set BrowserSubProcessPath based on app bitness at runtime
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", "CefSharp.BrowserSubprocess.exe")
            };

            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", assemblyName);

                return File.Exists(archSpecificPath) ? Assembly.LoadFile(archSpecificPath) : null;
            }

            return null;
        }
    }
}
