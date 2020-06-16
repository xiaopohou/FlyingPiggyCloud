using SixCloud.Store.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Store.Views
{
    /// <summary>
    /// LoginWebView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWebView : UserControl
    {
        public LoginWebView()
        {
            InitializeComponent();
            //Unloaded += (sender, e) =>
            //{
            //    mainContainer.Dispose();
            //};
            //mainContainer.NavigationCompleted += (sender, e) =>
            // {

            //     Application.Current.Dispatcher.Invoke(() =>
            //     {
            //         if (mainContainer.DataContext is LoginWebViewModel loginWebViewModel && loginWebViewModel.LoginUrl != mainContainer.Source.ToString())
            //         {
            //             mainContainer.Visibility = Visibility.Hidden;
            //         }
            //         else
            //         {
            //             loadingView.Visibility = Visibility.Hidden;
            //         }
            //     });
            // };
            mainContainer.Navigated += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (mainContainer.DataContext is LoginWebViewModel loginWebViewModel && loginWebViewModel.LoginUrl != mainContainer.Source.ToString())
                    {
                        mainContainer.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        loadingView.Visibility = Visibility.Hidden;
                    }
                });
            };
            DataContextChanged += (sender, e) =>
            {
                if (e.NewValue is LoginWebViewModel loginWebViewModel)
                {
                    mainContainer.Source = new Uri(loginWebViewModel.LoginUrl);
                }
            };
        }
    }
}
