using SixCloudCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SixCloudCore.Views.UserControls
{
    /// <summary>
    /// OfflineTaskListView.xaml 的交互逻辑
    /// </summary>
    public partial class OfflineTaskListView : UserControl
    {
        public OfflineTaskListView()
        {
            InitializeComponent();
            LazyLoadEventHandler = new ScrollChangedEventHandler(LazyLoad);
            LazyLoadEventHandler?.Invoke(this, null);
        }

        private ScrollChangedEventHandler LazyLoadEventHandler;

        private async void LazyLoad(object sender, ScrollChangedEventArgs e)
        {
            lock (LazyLoadEventHandler)
            {
                LazyLoadEventHandler -= LazyLoad;
            }
            //懒加载的业务代码
            //await (DataContext as OfflineTaskViewModel)?.LazyLoad();
            LazyLoadEventHandler += LazyLoad;
        }


        private void OfflineList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer viewer)
            {
                double bottomOffset = (viewer.ExtentHeight - viewer.VerticalOffset - viewer.ViewportHeight) / viewer.ExtentHeight;
                if (viewer.VerticalOffset > 0 && bottomOffset < 0.3)
                {
                    LazyLoadEventHandler?.Invoke(sender, e);
                }
            }
        }
    }
}
