using SixCloudCore.ViewModels;
using CustomControls.Controls;
using System;

namespace SixCloudCore.Views
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
