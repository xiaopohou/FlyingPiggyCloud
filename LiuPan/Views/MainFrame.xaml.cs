using QingzhenyunApis.EntityModels;
using SixCloud.ViewModels;
using CustomControls.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
            if (recoveryInfo != null)
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
            MainFrameViewModel mainFrameViewModel = new MainFrameViewModel(currentUser);
            DataContext = mainFrameViewModel;
            if (mainFrameViewModel.MainContainerContent is FileListViewModel fileListViewModel)
            {
                ThreadPool.QueueUserWorkItem(async(_) =>
                {
                    await fileListViewModel.NavigateByPath("/");
                });
            }
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
            if (e.NewValue is FileListViewModel)
            {
                FileListContainer.Visibility = Visibility.Visible;
                RecoveryBoxContainer.Visibility = Visibility.Collapsed;
            }
            else if (e.NewValue is RecoveryBoxViewModel)
            {
                FileListContainer.Visibility = Visibility.Collapsed;
                RecoveryBoxContainer.Visibility = Visibility.Visible;
            }
            else
            {
                return;
            }
            MainContainer.BeginAnimation(OpacityProperty, appearAnimation);
            MainContainerTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
        }

        private void LightButton_Click(object sender, RoutedEventArgs e)
        {
            UserInformationMenu.Visibility = Visibility.Visible;
        }

        private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (UserInformationMenu.Visibility == Visibility.Visible)
            {
                UserInformationMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void UserInformationMenu_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void FileListContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //如果不是从文件列表Raise的事件，忽略
            if (e.OriginalSource is ListView)
            {
                if (e.AddedItems.Count == 0)
                {
                    InfoView.BeginAnimation(OpacityProperty, new DoubleAnimation(0d, new Duration(TimeSpan.FromMilliseconds(300d))));
                }
                else
                {
                    InfoView.BeginAnimation(OpacityProperty, new DoubleAnimation(0d, 1d, new Duration(TimeSpan.FromMilliseconds(300d))));
                }
            }
        }

    }
}
