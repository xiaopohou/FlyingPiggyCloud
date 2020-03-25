using QingzhenyunApis.EntityModels;

namespace SixCloudCore.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public DependencyCommand PathNavigateCommand { get; private set; }

        public UserInformationViewModel UserInformation { get; set; }

        public ViewModelBase MainContainerContent { get; set; }

        private RecoveryBoxViewModel RecVM { get; set; }
        private FileListViewModel FileVM { get; set; }

        public TransferListViewModel TransferList { get; set; } = new TransferListViewModel();

        private async void PathNavigate(object parameter)
        {
            string path = parameter as string;
            if (path == "Recovery")
            {
                RecVM ??= new RecoveryBoxViewModel();
                MainContainerContent = RecVM;
                await RecVM.LazyLoad();
            }
            else
            {
                FileVM ??= new FileListViewModel();
                MainContainerContent = FileVM;
                await FileVM.NavigateByPath("/" + path, true);
            }
            OnPropertyChanged(nameof(MainContainerContent));
        }

        public MainFrameViewModel(UserInformation currentUser)
        {
            PathNavigateCommand = new DependencyCommand(PathNavigate, DependencyCommand.AlwaysCan);
            UserInformation = new UserInformationViewModel(currentUser);
            MainContainerContent = new FileListViewModel();
        }
    }
}
