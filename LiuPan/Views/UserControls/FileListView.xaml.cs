using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Views.UserControls
{
    /// <summary>
    /// FileListView.xaml 的交互逻辑
    /// </summary>
    public partial class FileListView : UserControl
    {
        public static readonly RoutedEvent PageNavigatedEvent = EventManager.RegisterRoutedEvent("PageNavigated", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(FileListView));
        public event RoutedEventHandler PageNavigated
        {
            add
            {
                AddHandler(PageNavigatedEvent, value);
            }
            remove
            {
                RemoveHandler(PageNavigatedEvent, value);
            }
        }
        protected virtual void OnNavigated()
        {
            RoutedEventArgs args = new RoutedEventArgs(PageNavigatedEvent, this);
            RaiseEvent(args);
        }

        public FileListView()
        {
            InitializeComponent();
        }

    }
}
