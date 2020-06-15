using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SixCloud.Core.Views
{

    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : UserControl
    {
        public MainFrame()
        {
            InitializeComponent();
            //上下文菜单收回后隐藏黑色底幕
            if (FindResource("contextMenuHideTimeLine") is Storyboard contextMenuHideTimeLine)
            {
                contextMenuHideTimeLine.Completed += (sender, e) =>
                {
                    contextMenuBg.Visibility = Visibility.Collapsed;
                };
            }
        }

        private void TabControl_Click(object sender, RoutedEventArgs e)
        {
            //避免click其他按钮展开菜单
            if (!(e.OriginalSource is Button contextMenuButton) || contextMenuButton.Name != "contextMenuButton")
            {
                e.Handled = true;
            }
        }

        private void ContextMenu_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //避免点击菜单内元素导致菜单收回
            e.Handled = true;
        }

        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            contextMenuBg.Visibility = Visibility.Visible;
        }
    }
}
