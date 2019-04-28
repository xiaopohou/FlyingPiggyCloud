using SixCloud.Models;
using System.Collections.Generic;

namespace SixCloud.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public AsyncCommand PathNavigateCommand { get; private set; }

        public UserInformationViewModel UserInformation { get; set; }

        public FileListViewModel FileList { get; set; }

        public TransferListViewModel TransferList { get; set; } = new TransferListViewModel();

        private void PathNavigate(object parameter)
        {
            FileList = new FileListViewModel();
            string path = parameter as string;
            if (path == "Recovery")
            {
                ///#warning 这里的代码还没写完
                ///V1接口缺乏回收站实现
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
