using System.Reflection;
using System.Windows;

namespace SixCloud.Core.Views.Dialogs
{
    /// <summary>
    /// AboutDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
            VersionBlock.Text = "当前版本：" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
