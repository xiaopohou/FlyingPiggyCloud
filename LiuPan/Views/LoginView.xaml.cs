using SixCloud.ViewModels;
using SixCloudCustomControlLibrary.Controls;

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
            DataContext = new AuthenticationViewModel(this);
        }
    }
}
