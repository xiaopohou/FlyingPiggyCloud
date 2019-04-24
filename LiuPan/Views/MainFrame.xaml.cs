using SixCloud.Models;
using SixCloud.ViewModels;
using SixCloudCustomControlLibrary.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SixCloud.Views
{

    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : MetroWindow
    {
        private static UserInformation recoveryInfo;
        public static Window Recovery()
        {
            if(recoveryInfo!=null)
            {
                return new MainFrame(recoveryInfo);
            }
            else
            {
                return new LoginView();
            }
        }

        public static readonly DependencyProperty PageNavigateProperty = DependencyProperty.Register("PageNavigate", typeof(PageNavigate), typeof(MainFrame), new PropertyMetadata((sender, e) =>
        {
            if (sender is MainFrame mainFrame)
            {

            }
        }));
        public PageNavigate PageNavigate { get => (PageNavigate)GetValue(PageNavigateProperty); set => SetValue(PageNavigateProperty, value); }


        public MainFrame(UserInformation currentUser)
        {
            recoveryInfo = currentUser;
            InitializeComponent();
            DataContext = new MainFrameViewModel(currentUser);
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
