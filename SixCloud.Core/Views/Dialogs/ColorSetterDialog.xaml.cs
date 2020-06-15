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
using System.Windows.Shapes;

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
