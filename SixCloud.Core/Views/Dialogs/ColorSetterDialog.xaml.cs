using System.Windows;

namespace SixCloud.Core.Views.Dialogs
{
    /// <summary>
    /// ColorSetterDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ColorSetterDialog : Window
    {
        public ColorSetterDialog(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                Top += (e.PreviousSize.Height - e.NewSize.Height) / 2;
            }
        }
    }
}
