using SixCloudCustomControlLibrary.Controls;
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
using System.Windows.Shapes;

namespace SixCloud.Views
{

    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : MetroWindow
    {
        public static readonly DependencyProperty PageNavigateProperty = DependencyProperty.Register("PageNavigate", typeof(PageNavigate), typeof(MainFrame), new PropertyMetadata((sender, e) =>
        {
            if(sender is MainFrame mainFrame)
            {
                //    switch(mainFrame.PageNavigate)
                //    {
                //        case PageNavigate.Completed:
                //            mainFrame.MainContainer
                //    }
            }
        }));
        public PageNavigate PageNavigate { get => (PageNavigate)GetValue(PageNavigateProperty); set => SetValue(PageNavigateProperty, value); }


        public MainFrame()
        {
            InitializeComponent();
        }

        private void MainContainer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DoubleAnimation appearAnimation = new DoubleAnimation
            {
                From = 0d,
                To = 1d,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                From = 100,
                To = 0d,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            MainContainer.BeginAnimation(OpacityProperty, appearAnimation);
            MainContainerTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
        }
    }
}
