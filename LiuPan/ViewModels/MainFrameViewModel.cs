using SixCloud.Models;

namespace SixCloud.ViewModels
{
    internal sealed class MainFrameViewModel : FileSystemViewModel
    {
        public AsyncCommand PathNavigateCommand { get; private set; }

        public UserInformationViewModel UserInformation { get; set; }

        public FileListViewModel FileList { get; set; }

        private void PathNavigate(object parameter)
        {
            FileList = new FileListViewModel();
            string path = parameter as string;
            if (path == "Recovery")
            {
#warning 这里的代码还没写完
            }
            else
            {
                FileList.NavigateByPath("/" + path, true);
            }
            OnPropertyChanged("FileList");
        }
        private bool CanPathNavigate(object parameter)
        {
            return true;
        }

        public MainFrameViewModel(UserInformation currentUser)
        {
            PathNavigateCommand = new AsyncCommand(PathNavigate, CanPathNavigate);
            UserInformation = new UserInformationViewModel(currentUser);
            FileList = new FileListViewModel();
            FileList.NavigateByPath("/");
        }
    }
}
