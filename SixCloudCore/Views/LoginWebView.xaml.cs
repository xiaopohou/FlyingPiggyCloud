using SixCloudCore.ViewModels;
using SourceChord.FluentWPF;
using System;
using System.Windows;

namespace SixCloudCore.Views
{
    /// <summary>
    /// LoginWebView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWebView : AcrylicWindow
    {
        public LoginWebView()
        {
            InitializeComponent();
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

                 //if (e.IsLoading || false)
                 //{
                 //    Application.Current.Dispatcher.Invoke(() => mainContainer.Visibility = Visibility.Hidden);
                 //}
             };

            //mainContainer.NavigationStarting += (sender, e) =>
            //  {
            //      if (mainContainer.Source != e.Uri)
            //      {
            //          mainContainer.Visibility = System.Windows.Visibility.Hidden;
            //      }
            //  };
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            mainContainer.Dispose();
        }
    }
}
