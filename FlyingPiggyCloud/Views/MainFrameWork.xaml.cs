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
    /// MainFrameWork.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrameWork : Window
    {
        //应有的用于绑定的对象：一个用户信息、一个页控制器、一个渲染页
        public MainFrameWork()
        {
            InitializeComponent();
        }

        private void MinWinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWinButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)

            {

                DragMove();

            }
        }
    }
}
