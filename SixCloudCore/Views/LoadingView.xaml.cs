using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SixCloudCore.Views
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingView : Window
    {
        public LoadingView(Window owner) : this(owner, null)
        {

        }

        public LoadingView(Window owner, Action action) : base()
        {
            InitializeComponent();
            Owner = owner;
            Width = owner.Width;
            Height = owner.Height;
            //BlurHolder.Visual = owner;
            Activated += (sender, e) => action?.BeginInvoke((parameter) => Application.Current.Dispatcher.Invoke(() => Close()), null);
        }

        public LoadingView(Window owner, Action action, string friendlyText) : this(owner, action)
        {
            FriendlyText.Text = friendlyText;
        }

        //public LoadingView(Window owner, Action action, string friendlyText, AlignmentX x, AlignmentY y):this(owner,action,friendlyText)
        //{
        //    //BlurHolder.AlignmentX = x;
        //    //BlurHolder.AlignmentY = y;
        //}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
    }
}
