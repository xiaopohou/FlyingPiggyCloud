using SixCloud.Core.ViewModels;
using System.Windows.Controls;

namespace SixCloud.Core.Views.UserControls
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
        }

        private ScrollChangedEventHandler LazyLoadEventHandler;

        private async void LazyLoad(object sender, ScrollChangedEventArgs e)
        {
            lock (LazyLoadEventHandler)
            {
                LazyLoadEventHandler -= LazyLoad;
            }
            await (DataContext as OfflineTaskViewModel)?.LazyLoad();
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
