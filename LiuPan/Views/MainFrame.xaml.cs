using SixCloud.Models;
using SixCloud.ViewModels;
using SixCloudCustomControlLibrary.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            Activated += ActivatedHandler;
            void ActivatedHandler(object sender, EventArgs e)
            {
                Activated -= ActivatedHandler;
                new LoadingView(this, () =>
                {
                    Thread.Sleep(1000);
                    Task t = mainFrameViewModel.FileList.NavigateByPath("/");
                    t.Wait();
                }, "正在加载文件目录").Show();
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
    }
}
