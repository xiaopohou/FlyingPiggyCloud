using System;
using System.Windows;
using System.Windows.Input;

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
            Activated += (sender, e) => action?.BeginInvoke((parameter) => Application.Current.Dispatcher.Invoke(() => Close()), null);
        }

        public LoadingView(Window owner, Action action, string friendlyText) : this(owner, action)
        {
            FriendlyText.Text = friendlyText;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
    }
}
