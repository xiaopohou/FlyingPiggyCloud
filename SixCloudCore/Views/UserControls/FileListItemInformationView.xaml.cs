using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SixCloudCore.Views.UserControls
{
    /// <summary>
    /// FileListItemInformationView.xaml 的交互逻辑
    /// </summary>
    public partial class FileListItemInformationView : UserControl
    {
        public FileListItemInformationView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                BeginAnimation(HeightProperty, new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 150))));
            }
            else
            {
                BeginAnimation(HeightProperty, new DoubleAnimation(150, new Duration(new TimeSpan(0, 0, 0, 0, 150))));
            }
        }
    }
}
