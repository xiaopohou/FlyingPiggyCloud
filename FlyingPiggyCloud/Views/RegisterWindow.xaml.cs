using FlyingPiggyCloud.Controllers.Results.User;
using System;
using System.Collections.Generic;
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
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private Models.RegisterViewModel RegisterViewModel = new Models.RegisterViewModel();

        public RegisterWindow()
        {
            InitializeComponent();
            DataContext = RegisterViewModel;
            SendMessage.Click += RegisterViewModel.OnSendMessageButtonClick;
            Register.Click += RegisterViewModel.OnRegisterButtonClick;
            RegisterViewModel.RegisterSuccessful += RegistSuccessful;
        }

        private void PassWord_PasswordChanged(object sender, RoutedEventArgs e)
        {
            RegisterViewModel.PassWord = PassWord.Password;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RegistSuccessful(UserInformation e)
        {
            Close();
        }
    }
}
