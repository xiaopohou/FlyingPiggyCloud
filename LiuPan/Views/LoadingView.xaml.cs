using System;
using System.Windows;

namespace SixCloud.Views
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingView : Window
    {
        public LoadingView(Window owner, Action action) : base()
        {
            InitializeComponent();
            Owner = owner;
            Width = owner.Width;
            Height = owner.Height;
            BlurHolder.Visual = owner;
            action?.BeginInvoke((parameter) => App.Current.Dispatcher.Invoke(() => Close()), null);
        }

        public LoadingView(Window owner,Action action,string friendlyText):this(owner,action)
        {
            FriendlyText.Text = friendlyText;
        }
    }
}
