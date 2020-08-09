using SixCloud.Core.Views;
using SourceChord.FluentWPF;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    public sealed class MainFrameViewModel : ViewModelBase
    {
        public async Task InitializeComponent()
        {
            LoadingElementVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(LoadingElementVisibility));
            if (Environment.OSVersion.Version >= new Version(6, 2))
            {
                MainFrameWindow = new AcrylicWindow
                {
                    AcrylicWindowStyle = AcrylicWindowStyle.NoIcon,
                };
            }
            else
            {
                MainFrameWindow = new Window
                {
                    WindowStyle = WindowStyle.ToolWindow
                };
            }

            MainFrameWindow.MinHeight = 720;
            MainFrameWindow.MinWidth = 800;
            MainFrameWindow.Title = FindLocalizationResource("Lang-Slogan");
            MainFrameWindow.DataContext = this;
            MainFrameWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainFrameWindow.Content = new MainFrame();
            MainFrameWindow.Show();

            await FileVM.NavigateByPath("/");
            LoadingElementVisibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(LoadingElementVisibility));
        }

        public Visibility LoadingElementVisibility { get; private set; }

        public Window MainFrameWindow { get; private set; }

        public RecoveryBoxViewModel RecVM { get; set; } = new RecoveryBoxViewModel();

        public FileListViewModel FileVM { get; set; } = new FileListViewModel();

        public TransferListViewModel TransferList { get; private set; } = new TransferListViewModel();

        public OfflineTaskViewModel OfflineTask { get; private set; } = new OfflineTaskViewModel();

        public UserInformationViewModel UserInformation { get; private set; } = new UserInformationViewModel();
    }
}
