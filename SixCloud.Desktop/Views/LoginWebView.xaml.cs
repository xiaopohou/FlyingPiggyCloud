using CefSharp;
using SixCloud.Desktop.ViewModels;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Desktop.Views
{
    /// <summary>
    /// LoginWebView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWebView : UserControl
    {
        public LoginWebView()
        {
            InitializeComponent();
            Unloaded += (sender, e) =>
            {
                mainContainer.Dispose();
            };
            mainContainer.MenuHandler = new MenuHandler();
            mainContainer.LoadingStateChanged += (sender, e) =>
             {

                 Application.Current.Dispatcher.Invoke(() =>
                 {
                     if (mainContainer.DataContext is LoginWebViewModel loginWebViewModel && loginWebViewModel.LoginUrl != mainContainer.Address)
                     {
                         mainContainer.Visibility = Visibility.Hidden;
                     }
                     else
                     {
                         loadingView.Visibility = Visibility.Hidden;
                     }
                 });
             };
        }

        private class MenuHandler : IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
            {

            }

            public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }
    }
}
