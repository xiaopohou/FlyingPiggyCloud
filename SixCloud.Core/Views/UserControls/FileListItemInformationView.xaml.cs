using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SixCloud.Core.Views.UserControls
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
                BeginAnimation(HeightProperty, new DoubleAnimation(450, new Duration(new TimeSpan(0, 0, 0, 0, 150))));
            }
        }
    }
}
