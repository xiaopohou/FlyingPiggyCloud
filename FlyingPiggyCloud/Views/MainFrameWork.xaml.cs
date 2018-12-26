using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Arthas;
using Arthas.Controls.Metro;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// MainFrameWork.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrameWork : MetroWindow, INotifyPropertyChanged
    {
        //应有的用于绑定的对象：一个用户信息、一个页控制器、一个渲染页
        public MainFrameWork()
        {
            CurrentPage = Controllers.RegistryManager.DefaultPage;
            UserInformationModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.UserInformationModel>(Controllers.RegistryManager.CurrentUserInformation);
            InitializeComponent();
            DataContext = this;
            UserGrid.DataContext = UserInformationModel;
            BindingOperations.SetBinding(MainFrame, ContentProperty, new Binding
            {
                Path = new PropertyPath("Page"),
                Mode = BindingMode.TwoWay
            });
        }

        ~MainFrameWork()
        {
            Controllers.RegistryManager.DefaultPage = CurrentPage;
        }

        /// <summary>
        /// 与左边栏绑定
        /// </summary>
        public PageNavigate CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                Navigate(value);
                OnPropertyChanged("CurrentPage");
            }
        }
        private PageNavigate currentPage;

        /// <summary>
        /// 导航页的内容
        /// </summary>
        public Page Page { get; set; }

        public Models.UserInformationModel UserInformationModel { get; set; }

        private DownloadingListPage DownloadingList = new DownloadingListPage();

        private RecoveryBoxPage RecoveryBox = new RecoveryBoxPage();

        private CompletedListPage CompletedList = new CompletedListPage();

        private void Navigate(PageNavigate pageNavigate)
        {
            switch (pageNavigate)
            {
                case PageNavigate.Root:
                    Page = new FilesListPage("/");
                    OnPropertyChanged("Page");
                    break;
                case PageNavigate.Images:
                    Page = new FilesListPage("/Images");
                    OnPropertyChanged("Page");
                    break;
                case PageNavigate.Videos:
                    Page = new FilesListPage("/Videos");
                    OnPropertyChanged("Page");
                    break;
                case PageNavigate.Uploading:
                    Page = new UploadingListPage();
                    OnPropertyChanged("Page");
                    break;
                case PageNavigate.Downloading:
                    Page = DownloadingList;
                    OnPropertyChanged("Page");
                    break;
                case PageNavigate.Completed:
                    Page = CompletedList;
                    OnPropertyChanged("Page");
                    break;
                case PageNavigate.RecoveryBox:
                    Page = RecoveryBox;
                    OnPropertyChanged("Page");
                    break;
                default:
                    Page = new FilesListPage("/");
                    break;
            }
        }

        private void MinWinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWinButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.LeftButton == MouseButtonState.Pressed)

            {

                DragMove();

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class LeftRadioButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PageNavigate s = (PageNavigate)value;
            return s == (PageNavigate)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (PageNavigate)int.Parse(parameter.ToString());
        }
    }

    public enum PageNavigate
    {
        Root,
        Images,
        Videos,
        Uploading,
        Downloading,
        Completed,
        RecoveryBox
    }
}
