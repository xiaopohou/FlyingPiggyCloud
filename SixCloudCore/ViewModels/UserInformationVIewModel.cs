//using Exceptionless;
using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using QingzhenyunApis.Utils;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using SixCloudCore.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SixCloudCore.ViewModels
{
    internal sealed class UserInformationViewModel : ViewModelBase
    {
        private readonly Authentication authentication = new Authentication();

        public ImageSource Icon { get; set; }

        public double AvailableRate { get; set; }

        public string FrendlySpaceCapacity { get; set; }

        public string Name { get; set; }

        public UserInformationViewModel(UserInformation currentUser)
        {
            ChangeUserNameCommand = new DependencyCommand(ChangeUserName, DependencyCommand.AlwaysCan);
            ChangePasswordCommand = new DependencyCommand(ChangePassword, DependencyCommand.AlwaysCan);
            LogoutCommand = new DependencyCommand(Logout, DependencyCommand.AlwaysCan);
            ParseInformation(currentUser);
        }

        private void ParseInformation(UserInformation currentUser)
        {
            string icon = null;
            try
            {
                icon = currentUser.Icon;
                if (string.IsNullOrEmpty(icon) || icon == "default.jpg" || icon == "default")
                {
                    icon = "http://qc.cdorey.net/default.jpg";
                }
                Icon = new BitmapImage(new Uri(icon));

            }
            catch (UriFormatException ex)
            {
                //检查导致解析头像崩溃的原因
                ex.ToExceptionless().AddObject(icon).AddObject(currentUser).Submit();
            }
            try
            {
                AvailableRate = currentUser.SpaceUsed * 100 / currentUser.SpaceCapacity;
            }
            catch (Exception)
            {
                AvailableRate = 100;
            }
            FrendlySpaceCapacity = $"总计：{Calculators.SizeCalculator(currentUser.SpaceCapacity)}{Environment.NewLine}已用：{Calculators.SizeCalculator(currentUser.SpaceUsed)}";
            Name = currentUser.Name;
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(AvailableRate));
            OnPropertyChanged(nameof(FrendlySpaceCapacity));
            OnPropertyChanged(nameof(Name));
        }

        #region Commands
        public DependencyCommand ChangeUserNameCommand { get; set; }
        private async void ChangeUserName(object parameter)
        {
            TextInputDialog.Show(out string newUserName, "请输入新用户名", "更改用户名");
            if (!string.IsNullOrWhiteSpace(newUserName))
            {
                var x = await Task.Run(() => authentication.ChangeUserName(newUserName));
                if (x.Success)
                {
                    ParseInformation((await Task.Run(() => authentication.GetUserInformation())).Result);
                }
                else
                {
                    MessageBox.Show($"未能成功修改用户名，原因：{x.Message}", "更改失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public DependencyCommand ChangePasswordCommand { get; set; }
        private void ChangePassword(object parameter)
        {
            new ChangePasswordView().ShowDialog();
        }

        public DependencyCommand LogoutCommand { get; set; }
        private void Logout(object parameter)
        {
            LocalProperties.Token = "";
            LocalProperties.IsAutoLogin = false;
            Application.Current.Shutdown();
        }
        #endregion
    }
}
