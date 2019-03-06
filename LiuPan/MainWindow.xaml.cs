using SixCloudCustomControlLibrary.Controls;
using System.Windows;
using System.Windows.Media;

namespace SixCloud
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Views.LoginView().Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Current.Resources["PrimaryHueMidBrush"] = new SolidColorBrush(new Color { A = 255, R = 0, G = 128, B = 128 });
        }
    }
}
