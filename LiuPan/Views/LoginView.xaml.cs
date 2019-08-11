using SixCloud.ViewModels;
using SixCloudCustomControlLibrary.Controls;
using System;

namespace SixCloud.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : MetroWindow
    {
        public LoginView()
        {
            InitializeComponent();
            AuthenticationViewModel dc = new AuthenticationViewModel(this);
            DataContext = dc;
            if (dc.IsAutoSignIn)
            {
                Activated += SignIn;

                void SignIn(object sender, EventArgs e)
                {
                    Activated -= SignIn;
                    dc.SignInCommand?.Execute(null);
                }
            }
        }
    }
}
