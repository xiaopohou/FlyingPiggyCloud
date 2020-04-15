using SourceChord.FluentWPF;

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
            mainContainer.NavigationStarting += (sender, e) =>
              {
                  if (mainContainer.Source != e.Uri)
                  {
                      mainContainer.Visibility = System.Windows.Visibility.Hidden;
                  }
              };
        }
    }
}
