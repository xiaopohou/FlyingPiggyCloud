using SixCloud.Controllers;
using SixCloud.Models;
using SixCloud.Views;
using System.Collections;
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

    internal class LogoutOthersViewModels : ViewModelBase
    {
        private readonly LogoutOthersView View;

        public OtherDeviceViewModels[] DeviceList { get; private set; }

        public string[] DevicesSSID { get; private set; }

        #region LogoutOthersCommand
        public DependencyCommand LogoutOthersCommand { get; set; }
        public void LogoutOthers(object parameter)
        {
            if (parameter is IList devices)
            {
                DevicesSSID = new string[devices.Count];
                for (int i = 0; i < devices.Count; i++)
                {
                    DevicesSSID[i] = ((OtherDeviceViewModels)devices[i]).SSID;
                }
                View.Close();
            }
        }
        #endregion

        #region CancelCommand
        public DependencyCommand CancelCommand { get; set; }
        public void Cancel(object parameter)
        {
            App.Current.Shutdown();
        }
        #endregion

        public LogoutOthersViewModels(GenericResult<OnlineDeviceList> onlineDeviceList)
        {
            LogoutOthersCommand = new DependencyCommand(LogoutOthers, DependencyCommand.AlwaysCan);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
            DeviceList = new OtherDeviceViewModels[onlineDeviceList.Result.Online.Length];
            for (int i = 0; i < DeviceList.Length; i++)
            {
                DeviceList[i] = new OtherDeviceViewModels(onlineDeviceList.Result.Online[i]);
            }
            View = new LogoutOthersView
            {
                DataContext = this
            };
        }

        public void ShowDialog()
        {
            View?.ShowDialog();
        }

        internal class OtherDeviceViewModels
        {
            private readonly OnlineClient clientModel;

            public string Device => clientModel.Device;

            public string LoginTime => Calculators.UnixTimeStampConverter(clientModel.LoginTime);

            public string RefreshTime => Calculators.UnixTimeStampConverter(clientModel.RefreshTime);

            public string SSID => clientModel.SSID;

            public string Status => clientModel.Status.ToString();

            public OtherDeviceViewModels(OnlineClient onlineClient)
            {
                clientModel = onlineClient;
            }
        }
    }
}
