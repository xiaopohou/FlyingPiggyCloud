using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class MainFrameWork : Window, INotifyPropertyChanged
    {
        //应有的用于绑定的对象：一个用户信息、一个页控制器、一个渲染页
        public MainFrameWork()
        {
            InitializeComponent();
        }

        private enum PageNavigate
        {
            Root,
            Images,
            Videos,
            Uploading,
            Downloading,
            Completed,
            RecoveryBox
        }

        /// <summary>
        /// 与左边栏绑定
        /// </summary>
        private PageNavigate CurrentPage { get; set; }

        private Page Page = new FilesListPage();

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
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.LeftButton == MouseButtonState.Pressed)

            {

                DragMove();

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
