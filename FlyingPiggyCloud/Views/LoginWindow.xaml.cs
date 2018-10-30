using FlyingPiggyCloud.Controllers.Results.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ViewModels.LoginByPasswordProgressViewModel LoginByPasswordProgressViewModel = new ViewModels.LoginByPasswordProgressViewModel();

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = LoginByPasswordProgressViewModel;
            Login.Click += LoginByPasswordProgressViewModel.OnLoginButtonClick;
            LoginByPasswordProgressViewModel.LoginSuccessful += LoginSuccessful;
        }

        private void LoginSuccessful(UserInformation e) => Close();

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Registering_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPassword = new ForgotPassword();
            forgotPassword.ShowDialog();
        }
    }
}
