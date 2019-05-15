using SixCloud.Models;

namespace SixCloud.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public AsyncCommand PathNavigateCommand { get; private set; }

        public UserInformationViewModel UserInformation { get; set; }

        public ViewModelBase MainContainerContent { get; set; }

        public TransferListViewModel TransferList { get; set; } = new TransferListViewModel();

        private async void PathNavigate(object parameter)
        {
            string path = parameter as string;
            if (path == "Recovery")
            {
                RecoveryBoxViewModel x = new RecoveryBoxViewModel();
                x.LazyLoad();
                MainContainerContent = x;
            }
            else if (path == "Search")
            {
#warning 这里的代码还没写完
            }
            else
            {
                FileListViewModel x = new FileListViewModel();
                await x.NavigateByPath("/" + path, true);
                MainContainerContent = x;
            }
            OnPropertyChanged("MainContainerContent");
        }
        private bool CanPathNavigate(object parameter)
        {
            return true;
        }

        public MainFrameViewModel(UserInformation currentUser)
        {
            PathNavigateCommand = new AsyncCommand(PathNavigate, CanPathNavigate);
            UserInformation = new UserInformationViewModel(currentUser);
            MainContainerContent = new FileListViewModel();
            //FileList.NavigateByPathAsync("/");
        }
    }
}
