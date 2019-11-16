using QingzhenyunApis.EntityModels;
using SixCloud.Controllers;
using SixCloud.Models;
using SixCloud.Views;
using System.Collections;

namespace SixCloud.ViewModels
{
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
