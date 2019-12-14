using QingzhenyunApis.EntityModels;
using SixCloud.Models;
using System.Threading.Tasks;

namespace SixCloud.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public DependencyCommand PathNavigateCommand { get; private set; }

        public UserInformationViewModel UserInformation { get; set; }

        public ViewModelBase MainContainerContent { get; set; }
        private RecoveryBoxViewModel recVM { get; set; }
        private FileListViewModel fileVM { get; set; }

        public TransferListViewModel TransferList { get; set; } = new TransferListViewModel();

        private async void PathNavigate(object parameter)
        {
            string path = parameter as string;
            if (path == "Recovery")
            {
                if(recVM==null)
                {
                    recVM = new RecoveryBoxViewModel();
                }
                MainContainerContent = recVM;
#warning 此处需要修理
                recVM.LazyLoad();
            }
            else
            {
                if(fileVM==null)
                {
                    fileVM = new FileListViewModel();
                }
                MainContainerContent = fileVM;
                await fileVM.NavigateByPath("/" + path, true);
            }
            OnPropertyChanged(nameof(MainContainerContent));
        }
        private bool CanPathNavigate(object parameter)
        {
            return true;
        }

        public MainFrameViewModel(UserInformation currentUser)
        {
            PathNavigateCommand = new DependencyCommand(PathNavigate, CanPathNavigate);
            UserInformation = new UserInformationViewModel(currentUser);
            fileVM = new FileListViewModel();
            MainContainerContent = fileVM;
            //FileList.NavigateByPathAsync("/");
        }
    }
}
